import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import {
  BodyCard,
  SectionTitle,
  WepInput,
  WepNumber,
  WepTextarea,
  Button,
  FileDrop,
} from '../../components';
import './create-project.scss';
import ProjectApi from '../../api/ProjectApi';

const CreationStatus = {
  NOT_STARTED: 'NOT_STARTED',
  STARTED: 'STARTED',
  DONE: 'DONE',
  ERROR: 'ERROR',
};

class CreateProject extends Component {
  constructor(props) {
    super(props);
    this.state = {
      title: '',
      goal: 0,
      openGoal: false,
      description: '',
      shippingInstructions: '',
      addrName: '',
      addr1: '',
      addr2: '',
      addr3: '',
      addrCity: '',
      addrState: '',
      addrZip: '',
      printingInstructions: '',
      thumb: null,
      projectId: null,
      uploadStatus: CreationStatus.NOT_STARTED,
    };
  }

  handleFormChange = ev => {
    const { name, value } = ev.target;
    this.setState({ [name]: value });
  };

  handleThumbChange = newFiles => {
    if (newFiles && newFiles.length) {
      const [newThumb] = newFiles;
      // if (newThumb.name === thumb.name) {
      //   // Duplicate prevention
      //   // TODO: add alert to let user's know if a file was already added
      //   return;
      // }
      this.setState({ thumb: newThumb });
    }
  };

  handleSubmission = () => {
    const {
      title,
      goal,
      openGoal,
      description,
      shippingInstructions,
      addrName,
      addr1,
      addr2,
      addr3,
      addrCity,
      addrState,
      addrZip,
      printingInstructions,
      thumb,
    } = this.state;

    this.setState({ uploadStatus: CreationStatus.STARTED });

    const payload = {
      title,
      description,
      goal,
      shippingInstructions,
      printingInstructions,
      address: {
        attention: addrName,
        addressLine1: addr1,
        addressLine2: addr2,
        addressLine3: addr3,
        city: addrCity,
        state: addrState,
        zipCode: addrZip,
      },
      openGoal,
    };

    ProjectApi.create(payload).subscribe(
      project => {
        console.log('success,', project);
        const { id } = project;

        const data = new FormData();
        data.append('postedImage', thumb);

        ProjectApi.setThumbnail(id, data).subscribe(
          () => this.setState({ projectId: id, uploadStatus: CreationStatus.DONE }),
          err => {
            // TODO: better error handling and reporting
            console.error(err);
            this.setState({ uploadStatus: CreationStatus.ERROR });
          }
        );
      },
      err => {
        console.error(err);
        this.setState({ uploadStatus: CreationStatus.ERROR });
      }
    );
  };

  render() {
    const {
      title,
      goal,
      openGoal,
      description,
      shippingInstructions,
      addrName,
      addr1,
      addr2,
      addr3,
      addrCity,
      addrState,
      addrZip,
      printingInstructions,
      thumb,
      projectId,
      uploadStatus,
    } = this.state;

    if (uploadStatus === CreationStatus.DONE) {
      return <Redirect to={`/projects/${projectId}`} push />;
    }

    return (
      <BodyCard className="create-proj" centered>
        <h1>Create Project</h1>
        <hr />
        <SectionTitle title="Basic Information" />
        <div className="create-proj__section">
          <div className="create-proj__basic-info">
            <div className="create-proj__key-info">
              <div className="input-group">
                <label htmlFor="title">Name*</label>
                <WepInput
                  name="title"
                  id="title"
                  value={title}
                  placeholder="Project Name..."
                  handleChange={this.handleFormChange}
                />
              </div>
              <div className="input-group">
                <label htmlFor="goal">Goal*</label>
                <WepNumber
                  name="goal"
                  id="goal"
                  value={goal}
                  handleChange={this.handleFormChange}
                />
              </div>
              <div className="create-proj__open">
                <input type="checkbox" name="open" id="open" value={openGoal} />
                <label htmlFor="open">Open Goal?</label>
              </div>
            </div>
            <FileDrop
              className="create-proj__thumb"
              handleFiles={this.handleThumbChange}
              customMsg={thumb ? thumb.name : 'Click or drag to upload project image'}
            />
            <div className="create-proj__desc">
              <div className="input-group">
                <label htmlFor="description">Desription*</label>
                <WepTextarea
                  name="description"
                  id="description"
                  value={description}
                  handleChange={this.handleFormChange}
                />
              </div>
            </div>
          </div>
        </div>
        <SectionTitle title="Delivery Information" />
        <div className="create-proj__section">
          <div className="input-group">
            <label htmlFor="shippingInstructions">Instructions*</label>
            <WepTextarea
              name="shippingInstructions"
              id="shippingInstructions"
              value={shippingInstructions}
              handleChange={this.handleFormChange}
            />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addrName">Name*</label>
            <WepInput
              name="addrName"
              id="addrName"
              value={addrName}
              handleChange={this.handleFormChange}
            />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addr1">Address 1*</label>
            <WepInput name="addr1" id="addr1" value={addr1} handleChange={this.handleFormChange} />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addr2">Address 2</label>
            <WepInput name="addr2" id="addr2" value={addr2} handleChange={this.handleFormChange} />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addr3">Address 3</label>
            <WepInput name="addr3" id="addr3" value={addr3} handleChange={this.handleFormChange} />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addrCity">City*</label>
            <WepInput
              name="addrCity"
              id="addrCity"
              value={addrCity}
              handleChange={this.handleFormChange}
            />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addrState">State*</label>
            <WepInput
              name="addrState"
              id="addrState"
              value={addrState}
              handleChange={this.handleFormChange}
            />
          </div>
          <div className="input-group input-group--inline">
            <label htmlFor="addrZip">Zipcode*</label>
            <WepInput
              name="addrZip"
              id="addrZip"
              value={addrZip}
              handleChange={this.handleFormChange}
            />
          </div>
        </div>
        <SectionTitle title="Printing Information" />
        <div className="create-proj__section">
          <div className="input-group">
            <label htmlFor="printingInstructions">Instructions*</label>
            <WepTextarea
              name="printingInstructions"
              id="printingInstructions"
              value={printingInstructions}
              handleChange={this.handleFormChange}
            />
          </div>
        </div>
        <div className="body-card__actions">
          <Button
            type={Button.Type.PRIMARY}
            size={Button.Size.LARGE}
            className="body-card__action-right"
            onClick={this.handleSubmission}
            disabled={this.uploadStatus === CreationStatus.STARTED}
          >
            Create Project
          </Button>
        </div>
      </BodyCard>
    );
  }
}

export default CreateProject;
