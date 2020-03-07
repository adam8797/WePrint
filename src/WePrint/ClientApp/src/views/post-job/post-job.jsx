import React, { Component } from 'react';
import { BodyCard, Button, Table, WepInput, WepTextarea, WepDropdown } from '../../components';
import ArrowProgressBar from './components/job-progress';
import './post-job.scss';

class PostJob extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentStage: 0,
      jobType: '',
      title: '',
      biddingPeriod: '',
      materialType: '',
      materialColor: '',
      shippingLocation: '',
      description: '',
      fileData: [
        {
          filename: 'test.txt',
          size: '200MB',
          progress: 'Processing',
        },
      ],
    };
  }

  setJobType(jobType) {
    this.setState({ jobType });
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
      shippingLocation,
      description,
      fileData,
    } = this.state;
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

    const matOptions = [
      {
        displayName: '',
        value: '',
      },
      {
        displayName: 'The Red one',
        value: 'red',
      },
      {
        displayName: 'Maybe blue?',
        value: 'blue',
      },
    ];

    const fileUploadCols = [
      {
        Header: 'Filename',
        accessor: 'filename',
      },
      {
        Header: 'Size',
        accessor: 'size',
      },
      {
        Header: 'Progress',
        accessor: 'progress',
      },
    ];

    return (
      <>
        <ArrowProgressBar states={states} active={currentStage} />
        {currentStage === 0 && (
          <BodyCard centered className="post-job-page">
            <h2>Select Type of Job</h2>
            <div className="type-buttons">
              <Button
                type={jobType === 'FDM' ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType('FDM')}
              >
                FDM
              </Button>
              <Button
                type={jobType === 'SLA' ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType('SLA')}
              >
                SLA
              </Button>
              <Button
                type={jobType === 'Laser' ? Button.Type.SUCCESS : Button.Type.PRIMARY}
                size={Button.Size.LARGE}
                onClick={() => this.setJobType('Laser')}
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
                <WepInput
                  name="biddingPeriod"
                  id="biddingPeriod"
                  value={biddingPeriod}
                  placeholder="Bidding Period"
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
                  options={matOptions}
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
                  options={matOptions}
                  handleChange={ev => this.handleFormChange('materialColor', ev.target.value)}
                />
              </div>
              <div className="input-group input-group--wide">
                <label htmlFor="shippingLocation">Shipping Location:</label>
                <WepInput
                  name="shippingLocation"
                  id="shippingLocation"
                  value={shippingLocation}
                  placeholder="Location..."
                  handleChange={ev => this.handleFormChange('shippingLocation', ev.target.value)}
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
                onClick={() => this.advanceStage()}
              >
                Next
              </Button>
            </div>
          </BodyCard>
        )}
        {currentStage === 2 && (
          <BodyCard centered className="post-job-page">
            <h2>Upload Files</h2>
            <div className="file-drop-area">Drag Files Here!</div>
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
