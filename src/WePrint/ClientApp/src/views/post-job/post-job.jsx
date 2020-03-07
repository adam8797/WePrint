import React, { Component } from 'react';
import moment from 'moment';
import filesize from 'filesize';
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

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentStage: 2,
      jobType: '',
      title: '',
      biddingPeriod: '',
      materialType: '',
      materialColor: '',
      shippingLocation: '',
      description: '',
      files: [],
    };
  }

  setJobType(jobType) {
    this.setState({ jobType });
  }

  removeFile = i => {
    this.setState(prevState => ({
      files: [...prevState.files.slice(0, i), ...prevState.files.slice(i + 1)],
    }));
  };

  handleFileChange = newFiles => {
    const { files: currFiles } = this.state;
    const maxFiles = 5;
    if (
      currFiles.length >= maxFiles ||
      currFiles.length + newFiles.length > maxFiles ||
      newFiles.length > maxFiles
    ) {
      console.log("That's too many files!");
      return;
    }
    if (newFiles && newFiles.length)
      this.setState(prevState => ({ files: [...prevState.files, ...newFiles] }));
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

  handleFormChange(prop, value) {
    this.setState({ [prop]: value });
  }

  render() {
    const {
      currentStage,
      jobType,
      title,
      biddingPeriod,
      materialType,
      materialColor,
      // shippingLocation,
      description,
      files,
      // fileData,
    } = this.state;

    const fileData = files.map(file => ({
      filename: file.name,
      size: file.size,
      progress: 'uploading',
    }));

    const states = [
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
        accessor: 'filename',
        Cell: ({ row: { id } }) => {
          return (
            <Button
              icon="trash"
              size={Button.Size.SMALL}
              type={Button.Type.DANGER}
              onClick={() => this.removeFile(+id)}
            />
          );
        },
      },
      { Header: 'Filename', accessor: 'filename' },
      {
        Header: 'Size',
        accessor: 'size',
        Cell: ({ cell: { value } }) => filesize(value),
      },
      { Header: 'Progress', accessor: 'progress' },
    ];

    const jobFormValid = jobType && title && biddingPeriod && materialType && materialColor;

    return (
      <>
        <ArrowProgressBar states={states} active={currentStage} />
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
                <label htmlFor="title">Title:</label>
                <WepInput
                  name="title"
                  id="title"
                  value={title}
                  placeholder="Title here"
                  handleChange={ev => this.handleFormChange('title', ev.target.value)}
                />
              </div>
              <div className="input-group">
                <label htmlFor="title">Bidding Period:</label>
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
              {/* <div className="input-group input-group--wide">
                <label htmlFor="shippingLocation">Shipping Location:</label>
                <WepInput
                  name="shippingLocation"
                  id="shippingLocation"
                  value={shippingLocation}
                  placeholder="Location..."
                  handleChange={ev => this.handleFormChange('shippingLocation', ev.target.value)}
                />
              </div> */}
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
                disabled={!jobFormValid}
              >
                Next
              </Button>
            </div>
          </BodyCard>
        )}
        {currentStage === 2 && (
          <BodyCard centered className="post-job-page">
            <h2>Upload Files</h2>
            <FileDrop
              className="post-job-page__file-drop"
              handleFiles={this.handleFileChange}
              multiple
            />
            <div className="file-list">
              <Table columns={fileUploadCols} data={fileData} emptyMessage="No files added yet" />
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
