import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { isEmpty, includes } from 'lodash';

import {
  SectionTitle,
  WepInput,
  WepNumber,
  WepTextarea,
  Button,
  FileDrop,
  toastError,
  StatusView,
} from '../../../components';
import { USStates } from '../../../models/Enums';
import ProjectApi from '../../../api/ProjectApi';

const CreationStatus = {
  NOT_STARTED: 'NOT_STARTED',
  STARTED: 'STARTED',
  DONE: 'DONE',
  ERROR: 'ERROR',
};

function ProjectForm() {
  const { projId } = useParams();
  const { register, handleSubmit, errors, reset } = useForm();

  const [project, setProject] = useState(null);
  const [thumb, setThumb] = useState(null);
  const [uploadStatus, setUploadStatus] = useState(CreationStatus.NOT_STARTED);

  useEffect(() => {
    const sub = ProjectApi.get(projId).subscribe(
      projectData => {
        setProject(projectData);
        reset(projectData);
      },
      err => {
        console.error(err);
        toastError('Error loading project');
      }
    );
    return () => sub.unsubscribe();
  }, [projId, reset]);

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
    const payload = {
      ...form,
      address: {
        ...form.address,
        state: form.address.state.toUpperCase(),
      },
    };

    setUploadStatus(CreationStatus.STARTED);

    ProjectApi.correct(projId, payload).subscribe(
      projectData => {
        console.log('success,', projectData);

        if (thumb) {
          const data = new FormData();
          data.append('postedImage', thumb);

          ProjectApi.setThumbnail(projId, data).subscribe(
            () => setUploadStatus(CreationStatus.DONE),
            err => {
              console.error(err);
              toastError('There was an error setting the project thumbnail during update');
              setUploadStatus(CreationStatus.ERROR);
            }
          );
        }
      },
      err => {
        console.error(err);
        toastError('There was an error updating the project');
        setUploadStatus(CreationStatus.ERROR);
      }
    );
  };

  if (!project) {
    return <StatusView text="Loading Details" icon="sync" spin />;
  }

  return (
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
                error={!!errors.goal}
              />
              {errors.goal && <div className="input-group__error">Goal is required</div>}
            </div>
            <div className="create-proj__open">
              <input
                type="checkbox"
                name="openGoal"
                id="openGoal"
                defaultValue={false}
                ref={register}
              />
              <label htmlFor="openGoal">Open Goal?</label>
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
            error={!!errors.shippingInstructions}
          />
          {errors.shippingInstructions && (
            <div className="input-group__error">Shipping instructions are required</div>
          )}
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.attention">Name*</label>
          <WepInput
            name="address.attention"
            id="address.attention"
            register={register({ required: true })}
            error={!!errors.attention}
          />
          {errors.attention && <div className="input-group__error">&nbsp; Name is required</div>}
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.addressLine1">Address 1*</label>
          <WepInput
            name="address.addressLine1"
            id="address.addressLine1"
            register={register({ required: true })}
            error={!!errors.addr1}
          />
          {errors.addr1 && <div className="input-group__error">&nbsp; Address is required</div>}
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.addressLine2">Address 2</label>
          <WepInput name="address.addressLine2" id="address.addressLine2" register={register} />
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.addressLine3">Address 3</label>
          <WepInput name="address.addressLine3" id="address.addressLine3" register={register} />
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.city">City*</label>
          <WepInput
            name="address.city"
            id="address.city"
            register={register({ required: true })}
            error={!!errors.addrCity}
          />
          {errors.addrCity && <div className="input-group__error">&nbsp;City is required</div>}
        </div>
        <div className="input-group input-group--inline">
          <label htmlFor="address.state">State*</label>
          <WepInput
            name="address.state"
            id="address.state"
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
          <label htmlFor="address.zipCode">Zipcode*</label>
          <WepInput
            name="address.zipCode"
            id="address.zipCode"
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
          type={Button.Type.SUCCESS}
          htmlType="submit"
          size={Button.Size.LARGE}
          className="body-card__action-right"
          disabled={uploadStatus === CreationStatus.STARTED || !isEmpty(errors)}
        >
          Save Project
        </Button>
      </div>
    </form>
  );
}

export default ProjectForm;
