import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';
import moment from 'moment';
import JobApi from '../../api/JobApi';
import { JobStatus } from '../../models/Enums';
import JobModel from '../../models/JobModel';
import ArrowProgressBar from './components/job-progress';
import StageType from './components/stage-type';
import StageInfo from './components/stage-info';
import StageUpload from './components/stage-upload';
import StageProcess from './components/stage-process';
import StageSuccess from './components/stage-success';
import { WepPrompt } from '../../components/wep-prompt/wep-prompt';
import './post-job.scss';

const ProgressColors = {
  PRIMARY: '',
  SUCCESS: 'success',
  INFO: 'info',
  WARNING: 'warning',
  DANGER: 'danger',
};

const UploadStates = {
  PENDING: 'Pending...',
  UPLOADING: 'Uploading',
  ERROR: 'Error!',
  COMPLETE: 'Complete!',
  REMOVING: 'Removing',
  ERROR_REMOVING: 'Error Deleting',
};

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentStage: 0,
      jobId: '',
      printerType: '',
      name: '',
      biddingPeriod: '',
      materialType: '',
      materialColor: '',
      description: '',
      files: [],
      maxFiles: 5,
      isDirty: false,
    };
  }

  setPrinterType = printerType => {
    this.setState({ printerType, isDirty: true });
  };

  removeFile = fileName => {
    // update progress to removing
    // try api remove
    // update progress
    // delete from state
    const { jobId } = this.state;

    this.updateFileProgress(fileName, UploadStates.REMOVING, ProgressColors.WARNING, 100);

    JobApi.DeleteFile(jobId, fileName).subscribe({
      error: err => {
        this.updateFileProgress(fileName, UploadStates.ERROR_REMOVING, ProgressColors.DANGER, 100);
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
        progress: {
          color: ProgressColors.PRIMARY,
          label: 'Pending',
          percent: 0,
        },
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
    this.setState({ isDirty: true });

    JobApi.CreateFile(jobId, data, ProgressEvent => {
      const progress = Math.round((ProgressEvent.loaded / ProgressEvent.total) * 100);
      this.updateFileProgress(fileName, `${progress}%`, ProgressColors.PRIMARY, progress);
    }).subscribe({
      error: err => {
        this.updateFileProgress(fileName, UploadStates.ERROR, ProgressColors.DANGER, 100);
        console.error(err);
      },
      complete: () => {
        this.setState({ isDirty: false });
        this.updateFileProgress(fileName, UploadStates.COMPLETE, ProgressColors.SUCCESS, 100);
      },
    });
  };

  handleFormChange = ev => {
    const { name, value } = ev.target;
    this.setState({ [name]: value, isDirty: true });
  };

  advanceStage() {
    this.setState(prevState => ({ currentStage: prevState.currentStage + 1 }));
  }

  reverseStage() {
    this.setState(prevState => ({ currentStage: prevState.currentStage - 1 }));
  }

  submitForm() {
    const {
      jobId,
      printerType,
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
    job.printerType = printerType;
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
          this.setState(prevState => ({
            currentStage: prevState.currentStage + 1,
            isDirty: false,
          }));
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
          this.setState(prevState => ({
            currentStage: prevState.currentStage + 1,
            isDirty: false,
          }));
        },
      });
    }
  }

  openBidding() {
    const { jobId } = this.state;
    const job = new JobModel();
    job.id = jobId;
    job.status = JobStatus.BiddingOpen;
    JobApi.UpdateJob(jobId, job).subscribe({
      // TODO: add alerts for submission in progress and posting error
      error: console.error,
      complete: () => {
        this.setState(prevState => ({ currentStage: prevState.currentStage + 1 }));
      },
    });
  }

  render() {
    const {
      currentStage,
      jobId,
      printerType,
      name,
      biddingPeriod,
      materialType,
      materialColor,
      description,
      files,
      maxFiles,
      isDirty,
    } = this.state;
    const { history } = this.props;

    const stages = [
      { name: 'Job Type' },
      { name: 'Basic Info' },
      { name: 'Upload Files' },
      { name: 'Pre-Process' },
    ];

    // TODO: might want to change this to a concrete "status" prop of progress
    // so progress.label can still have percentages
    const uploadComplete = files
      .map(file => file.progress.label)
      .reduce((acc, label) => {
        return acc && label === UploadStates.COMPLETE;
      }, true);

    return (
      <>
        <ArrowProgressBar stages={stages} active={currentStage} />
        {currentStage === 0 && (
          <StageType
            printerType={printerType}
            setPrinterType={this.setPrinterType}
            advanceAction={() => this.advanceStage()}
          />
        )}
        {currentStage === 1 && (
          <StageInfo
            name={name}
            biddingPeriod={biddingPeriod}
            materialType={materialType}
            materialColor={materialColor}
            description={description}
            handleFormChange={this.handleFormChange}
            reverseAction={() => this.reverseStage()}
            advanceAction={() => this.submitForm()}
          />
        )}
        {currentStage === 2 && (
          <StageUpload
            name={name}
            handleFileChange={this.handleFileChange}
            files={files}
            maxFiles={maxFiles}
            uploadComplete={uploadComplete}
            reverseAction={() => this.reverseStage()}
            advanceAction={() => this.advanceStage()}
          />
        )}
        {currentStage === 3 && (
          <StageProcess
            jobId={jobId}
            files={files}
            reverseAction={() => this.reverseStage()}
            advanceAction={() => this.openBidding()}
          />
        )}
        {currentStage > 3 && <StageSuccess jobId={jobId} />}
        <WepPrompt
          when={isDirty}
          navigate={path => history.push(path)}
          messages={[
            'If you navigate away from this page, your changes will not be saved.',
            'Are you sure you wish to navigate away and lose your progress?',
          ]}
        />
      </>
    );
  }
}

PostJob.propTypes = {
  history: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object])
  ).isRequired,
};

export default withRouter(PostJob);
