import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { isEmpty, includes } from 'lodash';

import {
  BodyCard,
  SectionTitle,
  WepInput,
  WepNumber,
  WepTextarea,
  Button,
  FileDrop,
  toastError,
} from '../../components';
import { USStates } from '../../models/Enums';
import ProjectApi from '../../api/ProjectApi';

import './create-project.scss';

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
      thumb: null,
      projectId: null,
      uploadStatus: CreationStatus.NOT_STARTED,
    };
  }

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

  handleSubmission = form => {
    const { projectId, thumb } = this.state;
    const {
      title,
      description,
      goal,
      shippingInstructions,
      printingInstructions,
      attention,
      addr1,
      addr2,
      addr3,
      addrCity,
      addrState,
      addrZip,
      openGoal,
    } = form;

    this.setState({ uploadStatus: CreationStatus.STARTED });

    const payload = {
      title,
      description,
      goal,
      shippingInstructions,
      printingInstructions,
      address: {
        attention,
        addressLine1: addr1,
        addressLine2: addr2,
        addressLine3: addr3,
        city: addrCity,
        state: addrState.toUpperCase(),
        zipCode: addrZip,
      },
      openGoal,
    };

    // TODO: Use this to help get around creation errors with the thumbnail to prevent duplicates
    // TODO: It's currently broken however, the patch doesn't work
    if (projectId && false) {
      ProjectApi.correct(projectId, payload).subscribe(
        project => {
          console.log('success,', project);

          const data = new FormData();
          data.append('postedImage', thumb);

          ProjectApi.setThumbnail(projectId, data).subscribe(
            () => this.setState({ uploadStatus: CreationStatus.DONE }),
            err => {
              console.error(err);
              toastError('There was an error setting the project thumbnail during update');
              this.setState({ uploadStatus: CreationStatus.ERROR });
            }
          );
        },
        err => {
          console.error(err);
          toastError('There was an error updating the project');
          this.setState({ uploadStatus: CreationStatus.ERROR });
        }
      );
    } else {
      ProjectApi.create(payload).subscribe(
        project => {
          console.log('success,', project);
          const { id } = project;
          this.setState({ projectId: id });

          const data = new FormData();
          data.append('postedImage', thumb);

          ProjectApi.setThumbnail(id, data).subscribe(
            () => this.setState({ uploadStatus: CreationStatus.DONE }),
            err => {
              console.error(err);
              toastError('There was an error setting the project thumbnail during creation');
              this.setState({ uploadStatus: CreationStatus.ERROR });
            }
          );
        },
        err => {
          console.error(err);
          toastError(`There was an error creating the project`);
          this.setState({ uploadStatus: CreationStatus.ERROR });
        }
      );
    }
  };

  render() {
    const { thumb, projectId, uploadStatus } = this.state;

    const { register, handleSubmit, errors } = useForm();

    if (uploadStatus === CreationStatus.DONE) {
      return <Redirect to={`/project/${projectId}`} push />;
    }

    return (
      <BodyCard className="create-proj" centered>
        <h1>Create Project</h1>
        <hr />
        <form onSubmit={handleSubmit(this.handleSubmission)}>
          <SectionTitle title="Basic Information" />
          <div className="create-proj__section">
            <div className="create-proj__basic-info">
              <div className="create-proj__key-info">
                <div className="input-group">
                  <label htmlFor="title">Name*</label>
                  <WepInput
                    name="title"
                    register={register({ required: true })}
                    id="title"
                    value=""
                    placeholder="Project Name..."
                    error={!!errors.title}
                  />
                  {errors.title && <div className="input-group__error">Title is required</div>}
                </div>
                <div className="input-group">
                  <label htmlFor="goal">Goal*</label>
                  <WepNumber
                    name="goal"
                    register={register({ required: true, min: 0 })}
                    id="goal"
                    value={0}
                    error={!!errors.goal}
                  />
                  {errors.goal && <div className="input-group__error">Goal is required</div>}
                </div>
                <div className="create-proj__open">
                  <input
                    type="checkbox"
                    name="open"
                    id="open"
                    defaultValue={false}
                    ref={register({ required: true })}
                  />
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
                    register={register({ required: true })}
                    value=""
                    error={!!errors.description}
                  />
                  {errors.description && (
                    <div className="input-group__error">Description is required</div>
                  )}
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
                register={register({ required: true })}
                value=""
                error={!!errors.shippingInstructions}
              />
              {errors.shippingInstructions && (
                <div className="input-group__error">Shipping instructions are required</div>
              )}
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="attention">Name*</label>
              <WepInput
                name="attention"
                id="attention"
                register={register({ required: true })}
                value=""
                error={!!errors.attention}
              />
              {errors.attention && <div className="input-group__error">Name is required</div>}
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addr1">Address 1*</label>
              <WepInput
                name="addr1"
                id="addr1"
                value=""
                register={register({ required: true })}
                error={!!errors.addr1}
              />
              {errors.addr1 && <div className="input-group__error">Address is required</div>}
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addr2">Address 2</label>
              <WepInput name="addr2" id="addr2" value="" register={register} />
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addr3">Address 3</label>
              <WepInput name="addr3" id="addr3" value="" register={register} />
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addrCity">City*</label>
              <WepInput
                name="addrCity"
                id="addrCity"
                value=""
                register={register({ required: true })}
                error={!!errors.addrCity}
              />
              {errors.addrCity && <div className="input-group__error">City is required</div>}
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addrState">State*</label>
              <WepInput
                name="addrState"
                id="addrState"
                value=""
                register={register({
                  required: true,
                  validate: value => includes(USStates, value.toUpperCase()),
                })}
                error={!!errors.addr1}
              />
              {errors.addrState && (
                <div className="input-group__error">
                  Please enter a valid two character state key
                </div>
              )}
            </div>
            <div className="input-group input-group--inline">
              <label htmlFor="addrZip">Zipcode*</label>
              <WepInput
                name="addrZip"
                id="addrZip"
                value=""
                register={register({ required: true, minLength: 5, maxLength: 5, min: 0 })}
                error={!!errors.addrZip}
              />
              {errors.addrZip && (
                <div className="input-group__error">Please input a valid zip code</div>
              )}
            </div>
          </div>
          <SectionTitle title="Printing Information" />
          <div className="create-proj__section">
            <div className="input-group">
              <label htmlFor="printingInstructions">Instructions*</label>
              <WepTextarea
                name="printingInstructions"
                id="printingInstructions"
                value=""
                register={register({ required: true })}
                error={!!errors.printingInstructions}
              />
              {errors.printingInstructions && (
                <div className="input-group__error">Instructions are required</div>
              )}
            </div>
          </div>
          <div className="body-card__actions">
            <Button
              type={Button.Type.PRIMARY}
              htmlType="submit"
              size={Button.Size.LARGE}
              className="body-card__action-right"
              onClick={this.handleSubmission}
              disabled={this.uploadStatus === CreationStatus.STARTED || !isEmpty(errors)}
            >
              Create Project
            </Button>
          </div>
        </form>
      </BodyCard>
    );
  }
}

export default CreateProject;
