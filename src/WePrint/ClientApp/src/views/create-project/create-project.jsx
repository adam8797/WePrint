import React, { useState } from 'react';
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
  StatusView,
  AccountRestrictedView,
} from '../../components';
import { USStates } from '../../models/Enums';
import ProjectApi from '../../api/ProjectApi';

import './create-project.scss';
import UserApi from '../../api/UserApi';

const CreationStatus = {
  NOT_STARTED: 'NOT_STARTED',
  STARTED: 'STARTED',
  DONE: 'DONE',
  ERROR: 'ERROR',
};

function CreateProject() {
  const [thumb, setThumb] = useState(null);
  const [projectId, setProjectId] = useState(null);
  const [uploadStatus, setUploadStatus] = useState(CreationStatus.NOT_STARTED);
  const [user, setUser] = useState(null);

  const { register, handleSubmit, errors } = useForm();

  UserApi.CurrentUser().subscribe(u => {
    setUser(u);
  }, console.error);

  if (user === null) {
    return (
      <BodyCard>
        <StatusView text="Loading..." icon="sync" spin />
      </BodyCard>
    );
  }

  if (!user) {
    return (
      <BodyCard>
        <AccountRestrictedView />
      </BodyCard>
    );
  }

  const handleThumbChange = newFiles => {
    if (newFiles && newFiles.length) {
      const [newThumb] = newFiles;
      // if (newThumb.name === thumb.name) {
      //   // Duplicate prevention
      //   // TODO: add alert to let user's know if a file was already added
      //   return;
      // }
      setThumb(newThumb);
    }
  };

  const handleSubmission = form => {
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

    setUploadStatus(CreationStatus.STARTED);

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
    if (projectId) {
      ProjectApi.correct(projectId, payload).subscribe(
        project => {
          console.log('success,', project);

          const data = new FormData();
          data.append('postedImage', thumb);

          ProjectApi.setThumbnail(projectId, data).subscribe(
            () => setUploadStatus(CreationStatus.DONE),
            err => {
              console.error(err);
              toastError('There was an error setting the project thumbnail during update');
              setUploadStatus(CreationStatus.ERROR);
            }
          );
        },
        err => {
          console.error(err);
          toastError('There was an error updating the project');
          setUploadStatus(CreationStatus.ERROR);
        }
      );
    } else {
      ProjectApi.create(payload).subscribe(
        project => {
          console.log('success,', project);
          const { id } = project;
          setProjectId(id);

          const data = new FormData();
          data.append('postedImage', thumb);

          ProjectApi.setThumbnail(id, data).subscribe(
            () => setUploadStatus(CreationStatus.DONE),
            err => {
              console.error(err);
              toastError('There was an error setting the project thumbnail during creation');
              setUploadStatus(CreationStatus.ERROR);
            }
          );
        },
        err => {
          console.error(err);
          toastError(`There was an error creating the project`);
          setUploadStatus(CreationStatus.ERROR);
        }
      );
    }
  };

  if (uploadStatus === CreationStatus.DONE) {
    return <Redirect to={`/project/${projectId}`} push />;
  }

  return (
    <BodyCard className="create-proj" centered>
      <h1>Create Project</h1>
      <hr />
      <form onSubmit={handleSubmit(handleSubmission)}>
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
                <input type="checkbox" name="open" id="open" defaultValue={false} ref={register} />
                <label htmlFor="open">Open Goal?</label>
              </div>
            </div>
            <div>
              <FileDrop
                className="create-proj__thumb"
                handleFiles={handleThumbChange}
                customMsg={thumb ? thumb.name : 'Click or drag to upload project image'}
              />
            </div>
            <div className="create-proj__desc">
              <div className="input-group">
                <label htmlFor="description">Description*</label>
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
            {errors.attention && <div className="input-group__error">&nbsp; Name is required</div>}
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
            {errors.addr1 && <div className="input-group__error">&nbsp; Address is required</div>}
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
            {errors.addrCity && <div className="input-group__error">&nbsp;City is required</div>}
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
                &nbsp;Please enter a valid two character state key
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
              <div className="input-group__error">&nbsp;Please input a valid zip code</div>
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
            disabled={uploadStatus === CreationStatus.STARTED || !isEmpty(errors)}
          >
            Create Project
          </Button>
        </div>
      </form>
    </BodyCard>
  );
}

export default CreateProject;
