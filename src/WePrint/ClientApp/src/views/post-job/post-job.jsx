import React, { Component } from 'react';
import moment from 'moment';
import filesize from 'filesize';
import { Progress } from 'reactstrap';
import {
  BodyCard,
  Button,
  FileDrop,
  Table,
  WepInput,
  WepTextarea,
  WepDropdown,
} from '../../components';
import JobApi from '../../api/JobApi';
import { MaterialType, MaterialColor, PrinterType, JobStatus } from '../../models/Enums';
import JobModel from '../../models/JobModel';
import ArrowProgressBar from './components/job-progress';
import './post-job.scss';

const ProgressColors = {
  PRIMARY: '',
  SUCCESS: 'success',
  INFO: 'info',
  WARNING: 'warning',
  DANGER: 'danger',
};

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentStage: 0,
      jobId: '',
      jobType: '',
      name: '',
      biddingPeriod: '',
      materialType: '',
      materialColor: '',
      description: '',
      files: [],
      maxFiles: 5,
    };
  }

  setJobType(jobType) {
    this.setState({ jobType });
  }

  removeFile = fileName => {
    // update progress to removing
    // try api remove
    // update progress
    // delete from state
    const { jobId } = this.state;

    this.updateFileProgress(fileName, 'Removing', ProgressColors.WARNING, 100);

    JobApi.DeleteFile(jobId, fileName).subscribe({
      error: err => {
        this.updateFileProgress(fileName, 'Error Deleting!', ProgressColors.DANGER, 100);
        console.error(err);
      },
      complete: () => {
        this.setState(prevState => {
          const i = prevState.files.findIndex(f => f.fileName === fileName);
          return {
            files: [...prevState.files.slice(0, i), ...prevState.files.slice(i + 1)],
          };
        });
      },
    });
  };

  handleFileChange = newFiles => {
    const { files: currFiles, maxFiles } = this.state;
    if (
      currFiles.length >= maxFiles ||
      currFiles.length + newFiles.length > maxFiles ||
      newFiles.length > maxFiles
    ) {
      console.log("That's too many files!");
      return;
    }
    if (newFiles && newFiles.length) {
      const filesToAdd = newFiles.filter(
        // Duplicate prevention
        // TODO: add alert to let user's know if a file was already added
        newFile => currFiles.findIndex(f => f.fileName === newFile.name) === -1
      );
      const newStateFiles = filesToAdd.map(file => ({
        fileName: file.name,
        size: file.size,
        fileData: file,
        progress: 'Pending',
      }));

      this.setState(
        prevState => ({ files: [...prevState.files, ...newStateFiles] }),
        () => {
          // start uploading them after they've been added to the state
          newStateFiles.forEach(this.uploadFile);
        }
      );
    }
  };

  updateFileProgress = (fileName, label, color, percent) => {
    this.setState(prevState => {
      const files = [...prevState.files];
      const i = files.findIndex(f => f.fileName === fileName);
      if (i !== -1) {
        const oldFile = files[i];
        // file found
        // reminder splice happens IN PLACE
        files.splice(i, 1, {
          ...oldFile,
          progress: {
            ...oldFile.progress,
            label,
            color,
            percent,
          },
        });
      }
      return {
        files,
      };
    });
  };

  uploadFile = file => {
    const { jobId } = this.state;
    const { fileName, fileData } = file;

    const data = new FormData();
    data.append('file', fileData);

    this.updateFileProgress(fileName, '0%', ProgressColors.PRIMARY, 0);

    JobApi.CreateFile(jobId, data, ProgressEvent => {
      const progress = Math.round((ProgressEvent.loaded / ProgressEvent.total) * 100);
      this.updateFileProgress(fileName, `${progress}%`, ProgressColors.PRIMARY, progress);
    }).subscribe({
      next: () => {
        this.updateFileProgress(fileName, 'Uploaded', ProgressColors.PRIMARY, 100);
      },
      error: err => {
        this.updateFileProgress(fileName, 'Error!', ProgressColors.DANGER, 100);
        console.error(err);
      },
      complete: () => {
        this.updateFileProgress(fileName, 'Complete!', ProgressColors.SUCCESS, 100);
      },
    });
  };

  handleFormChange(prop, value) {
    this.setState({ [prop]: value });
  }

  advanceStage() {
    this.setState(prevState => ({ currentStage: prevState.currentStage + 1 }));
  }

  reverseStage() {
    this.setState(prevState => ({ currentStage: prevState.currentStage - 1 }));
  }

  submitForm() {
    const {
      jobId,
      jobType,
      name,
      biddingPeriod,
      materialType,
      materialColor,
      description,
    } = this.state;
    const bidClose = moment()
      .subtract(+biddingPeriod, 'days')
      .format();

    const job = new JobModel();
    job.name = name;
    job.status = JobStatus.PendingOpen;
    job.description = description;
    job.printerType = jobType;
    job.materialType = materialType;
    job.materialColor = materialColor;
    job.bidClose = bidClose;

    if (jobId) {
      // if we've already submitted, update the job
      job.id = jobId;
      JobApi.UpdateJob(jobId, job).subscribe({
        next: apiJob => {
          this.setState({ jobId: apiJob.id });
          // TODO: add alert to show the job has been updated
        },
        error: console.error,
        complete: () => {
          this.setState(prevState => ({ currentStage: prevState.currentStage + 1 }));
        },
      });
    } else {
      // otherwise create the job
      JobApi.CreateJob(job).subscribe({
        next: apiJob => {
          this.setState({ jobId: apiJob.id });
          // TODO: add alert that job has been created, ready for files
        },
        error: console.error,
        complete: () => {
          this.setState(prevState => ({ currentStage: prevState.currentStage + 1 }));
        },
      });
    }
  }

  render() {
    const {
      currentStage,
      jobType,
      name,
      biddingPeriod,
      materialType,
      materialColor,
      description,
      files,
      maxFiles,
    } = this.state;

    const stages = [
      {
        name: 'Job Type',
      },
      {
        name: 'Basic Info',
      },
      {
        name: 'Upload Files',
      },
      {
        name: 'Pre-Process',
      },
    ];

    const biddingOpts = [
      { displayName: '', value: '' },
      { displayName: '1 Day', value: '1' },
      { displayName: '2 Days', value: '2' },
      { displayName: '3 Days', value: '3' },
      { displayName: '4 Days', value: '4' },
      { displayName: '5 Days', value: '5' },
    ];

    const matTypeOpts = [
      { displayName: '', value: '' },
      ...Object.entries(MaterialType).map(([key, value]) => ({ displayName: value, value: key })),
    ];
    const matColorOpts = [
      { displayName: '', value: '' },
      ...Object.entries(MaterialColor).map(([key, value]) => ({ displayName: value, value: key })),
    ];

    const fileUploadCols = [
      {
        id: 'delete',
        accessor: 'fileName',
        Cell: ({ cell: { value: fileName } }) => (
          <Button
            icon="trash"
            size={Button.Size.SMALL}
            type={Button.Type.DANGER}
            onClick={() => this.removeFile(fileName)}
          />
        ),
      },
      {
        Header: 'Filename',
        accessor: 'fileName',
      },
      {
        Header: 'Size',
        accessor: 'size',
        Cell: ({ cell: { value } }) => filesize(value),
      },
      {
        Header: 'Progress',
        accessor: 'progress',
        Cell: ({ cell: { value: progress } }) => (
          <Progress color={progress.color} value={progress.percent}>
            {progress.label}
          </Progress>
        ),
      },
    ];
    // map state.files to proper table data and exclude excess
    const fileUploadData = files.map(({ fileName, size, progress }) => ({
      fileName,
      size,
      progress,
    }));

    const jobFormValid = jobType && name && biddingPeriod && materialType && materialColor;

    return (
      <>
        <ArrowProgressBar stages={stages} active={currentStage} />
        {currentStage === 0 && (
          <BodyCard centered className="post-job-page">
            <h2>Select Type of Job</h2>
            <div className="type-buttons">
              <Button
                type={jobType === PrinterType.FDM ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType(PrinterType.FDM)}
              >
                FDM
              </Button>
              <Button
                type={jobType === PrinterType.SLA ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType(PrinterType.SLA)}
              >
                SLA
              </Button>
              <Button
                type={jobType === PrinterType.LaserCut ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType(PrinterType.LaserCut)}
              >
                Laser
              </Button>
            </div>
            <div className="body-card__actions">
              <Button
                type={Button.Type.SUCCESS}
                className="body-card__action-right"
                onClick={() => this.advanceStage()}
                disabled={!jobType}
              >
                Next
              </Button>
            </div>
          </BodyCard>
        )}
        {currentStage === 1 && (
          <BodyCard centered className="post-job-page">
            <h2>Basic Info</h2>
            <div className="basic-info__form">
              <div className="input-group">
                <label htmlFor="name">Job Name:</label>
                <WepInput
                  name="name"
                  id="name"
                  value={name}
                  placeholder="Job name..."
                  handleChange={ev => this.handleFormChange('name', ev.target.value)}
                />
              </div>
              <div className="input-group">
                <label htmlFor="biddingPeriod">Bidding Period:</label>
                <WepDropdown
                  name="biddingPeriod"
                  id="biddingPeriod"
                  value={biddingPeriod}
                  placeholder="Bidding Period..."
                  options={biddingOpts}
                  handleChange={ev => this.handleFormChange('biddingPeriod', ev.target.value)}
                />
              </div>
              <div className="input-group">
                <label htmlFor="materialType">Material Type:</label>
                <WepDropdown
                  name="materialType"
                  id="materialType"
                  value={materialType}
                  placeholder="Select one..."
                  options={matTypeOpts}
                  handleChange={ev => this.handleFormChange('materialType', ev.target.value)}
                />
              </div>
              <div className="input-group">
                <label htmlFor="materialColor">Material Color:</label>
                <WepDropdown
                  name="materialColor"
                  id="materialColor"
                  value={materialColor}
                  placeholder="Select one..."
                  options={matColorOpts}
                  handleChange={ev => this.handleFormChange('materialColor', ev.target.value)}
                />
              </div>
              <div className="input-group input-group--wide">
                <label htmlFor="description">Description:</label>
                <WepTextarea
                  name="description"
                  id="description"
                  value={description}
                  placeholder="Description here"
                  handleChange={ev => this.handleFormChange('description', ev.target.value)}
                />
              </div>
            </div>
            <div className="body-card__actions">
              <Button
                type={Button.Type.DANGER}
                className="body-card__action-left"
                onClick={() => this.reverseStage()}
              >
                Back
              </Button>
              <Button
                type={Button.Type.SUCCESS}
                className="body-card__action-right"
                onClick={() => this.submitForm()}
                disabled={!jobFormValid}
              >
                Next
              </Button>
            </div>
          </BodyCard>
        )}
        {currentStage === 2 && (
          <BodyCard centered className="post-job-page">
            <h2>Upload Files to: {name}</h2>
            <FileDrop
              className="post-job-page__file-drop"
              handleFiles={this.handleFileChange}
              accept=".3mf, .stl"
              multiple
              disabled={files.length >= maxFiles}
              customMsg="Drag .3mf & .stl files here, or click to select files"
              disabledMsg={`Cannot add more files, max ${maxFiles} allowed`}
            />
            <div className="file-list">
              <Table
                columns={fileUploadCols}
                data={fileUploadData}
                emptyMessage="No files added yet"
              />
            </div>
            <div className="body-card__actions">
              <Button
                type={Button.Type.DANGER}
                className="body-card__action-left"
                onClick={() => this.reverseStage()}
              >
                Back
              </Button>
              <Button
                type={Button.Type.SUCCESS}
                className="body-card__action-right"
                onClick={() => this.advanceStage()}
                disabled={!files.length}
              >
                Next
              </Button>
            </div>
          </BodyCard>
        )}
        {currentStage === 3 && (
          <BodyCard centered className="post-job-page">
            <h2>Pre-Process</h2>
            <div className="body-card__actions">
              <Button
                type={Button.Type.DANGER}
                className="body-card__action-left"
                onClick={() => this.reverseStage()}
              >
                Back
              </Button>
              <Button type={Button.Type.SUCCESS} className="body-card__action-right">
                Done
              </Button>
            </div>
          </BodyCard>
        )}
      </>
    );
  }
}

export default PostJob;
