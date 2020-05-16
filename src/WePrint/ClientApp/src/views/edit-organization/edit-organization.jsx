import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useForm } from 'react-hook-form';
import { isEmpty, includes } from 'lodash';

import UserApi from '../../api/UserApi';
import OrgApi from '../../api/OrganizationApi';

import {
  FormGroup,
  BodyCard,
  WepInput,
  WepTextarea,
  FileDrop,
  Button,
  ButtonType,
  StatusView,
} from '../../components';

import './edit-organization.scss';
import { USStates } from '../../models/Enums';

function EditOrganization(props) {
  const { currentUser, users, returnCallback } = props;
  const { register, handleSubmit, errors } = useForm();

  let { organization } = props;
  if (!organization) {
    organization = { name: 'New Organization', address: {} };
  }

  let callback = returnCallback;
  if (!callback) {
    callback = org => {
      let loc = window.location.origin;
      if (org && org.id) {
        loc += `/organization/${org.id}`;
      }
      window.location.href = loc;
    };
  }

  // users
  const [newUser, setNewUser] = useState('');
  const [orgUsers, setOrgUsers] = useState(users || []);
  const [user, setUser] = useState(currentUser);

  // file
  const [logoUrl, setLogoUrl] = useState(organization.id && OrgApi.getAvatarUrl(organization.id));
  const [file, setFile] = useState(null);

  const [error, setError] = useState(false);

  if (!user) {
    UserApi.CurrentUser().subscribe(u => {
      // currently this information is not sent back we need to fix this!
      if (u.organization) {
        setError(true);
        return;
      }
      setUser(u);
      if (!orgUsers.length) {
        setOrgUsers([u]);
      }
    });
  }

  const addUser = () => {
    if (!newUser || users.filter(u => u.username === newUser).length) {
      return;
    }
    UserApi.GetUserByUsername(newUser).subscribe(u => {
      setOrgUsers([...orgUsers, u]);
    });
  };

  const removeUser = u => {
    setOrgUsers(orgUsers.filter(orgUser => orgUser.id !== u.id));
  };

  const saveOrg = form => {
    const org = organization;
    organization.name = form.name;
    organization.description = form.description;
    organization.address = {
      attention: form.attention,
      addressLine1: form.addressLine1,
      addressLine2: form.addressLine2,
      addressLine3: form.addressLine3,
      city: form.city,
      state: form.state.toUpperCase(),
      zipCode: form.zipCode,
    };
    organization.users = orgUsers.map(u => u.id);
    let data;
    if (file) {
      data = new FormData();
      data.append('upload', file);
    }

    if (organization.id) {
      OrgApi.replace(organization.id, org).subscribe(response => {
        if (data) {
          OrgApi.postAvatar(organization.id, data).subscribe(() => {
            callback(response);
          }, console.error);
        } else {
          callback(response);
        }
      }, console.error);
    } else {
      OrgApi.create(org).subscribe(response => {
        if (data) {
          OrgApi.postAvatar(response.id, data).subscribe(() => {
            callback(response);
          }, console.error);
        } else {
          callback(response);
        }
      }, console.error);
    }
  };

  const deleteOrg = () => {
    if (!organization.id) return;
    OrgApi.delete(organization.id).subscribe(callback, console.error);
  };

  const uploadFile = files => {
    setFile(files[0]);
    const reader = new FileReader();
    reader.onload = () => {
      setLogoUrl(reader.result);
    };
    reader.readAsDataURL(files[0]);
  };

  const getUserDisplay = u => {
    if (!u) return false;
    if (user && user.id === u.id) {
      return (
        <div className="edit-org__user">
          {u.firstName} {u.lastName} (You)
        </div>
      );
    }
    return (
      <div className="edit-org__user">
        {u.firstName} {u.lastName}
        <div className="edit-org__remove-icon">
          <FontAwesomeIcon
            icon="times"
            onClick={() => {
              removeUser(u);
            }}
          />
        </div>
      </div>
    );
  };

  if (error) {
    return (
      <BodyCard>
        <StatusView text="You already have an organization!" />
      </BodyCard>
    );
  }
  return (
    <BodyCard>
      <div className="edit-org">
        <div className="edit-org__header">{organization.name}</div>
        <hr />
        <form onSubmit={handleSubmit(saveOrg)}>
          <div className="edit-org__split-inline edit-org__split-inline--equal">
            <div>
              <FormGroup title="Organization Name*" help="What is the organization called?">
                <WepInput
                  name="name"
                  register={register({ required: true })}
                  value={organization.name}
                  id="name"
                  placeholder="Organization Name..."
                  error={!!errors.name}
                />
                {errors.name && <div className="edit-org__input-error">Name is required</div>}
              </FormGroup>
              <FormGroup title="Location" help="Where is the organization located?">
                <label htmlFor="attention">Attention*</label>
                <WepInput
                  name="attention"
                  register={register({ required: true })}
                  id="attention"
                  value={organization.address.attention}
                  error={!!errors.attention}
                />
                {errors.attention && (
                  <div className="edit-org__input-error">Attention is required</div>
                )}
                <label htmlFor="addressLine1">Address*</label>
                <WepInput
                  name="addressLine1"
                  register={register({ required: true })}
                  id="addressLine1"
                  value={organization.address.addressLine1}
                  error={!!errors.addressLine1}
                />
                {errors.addressLine1 && (
                  <div className="edit-org__input-error">Address is required</div>
                )}
                <WepInput
                  name="addressLine2"
                  register={register}
                  id="addressLine2"
                  value={organization.address.addressLine2}
                />
                <WepInput
                  name="addressLine3"
                  register={register}
                  id="addressLine3"
                  value={organization.address.addressLine3}
                />
                <label htmlFor="city">City*</label>
                <WepInput
                  name="city"
                  register={register({ required: true })}
                  id="city"
                  value={organization.address.city}
                  error={!!errors.city}
                />
                {errors.city && <div className="edit-org__input-error">City is required</div>}
                <label htmlFor="state">State*</label>
                <WepInput
                  name="state"
                  register={register({
                    required: true,
                    validate: value => includes(USStates, value.toUpperCase()),
                  })}
                  id="state"
                  value={organization.address.state}
                  error={!!errors.state}
                />
                {errors.state && (
                  <div className="edit-org__input-error">
                    Please enter a valid two-character state key
                  </div>
                )}
                <label htmlFor="zipCode">Zip*</label>
                <WepInput
                  name="zipCode"
                  register={register({ required: true, minLength: 5, maxLength: 5, min: 0 })}
                  id="zipCode"
                  htmlType="number"
                  value={organization.address.zipCode}
                  error={!!errors.zipCode}
                />
                {errors.zipCode && (
                  <div className="edit-org__input-error">Please enter a valid zip code</div>
                )}
              </FormGroup>
            </div>
            <div>
              <FormGroup
                title="Logo"
                help="The image that will be displayed next to the organization"
              >
                <div className="edit-org__split-inline">
                  {(logoUrl && (
                    <img className="edit-org__logo" src={logoUrl} alt="Organization Logo" />
                  )) || (
                    <div className="edit-org__logo edit-org__logo--blank">No Logo Uploaded</div>
                  )}
                  <FileDrop
                    handleFiles={uploadFile}
                    name="logo"
                    accept=".png, .jpg, .jpeg"
                    customMsg="Drag an image file here, or click to select one"
                  />
                </div>
              </FormGroup>
            </div>
          </div>
          <FormGroup title="Organization Bio*" help="A description of the organization">
            <WepTextarea
              name="description"
              register={register({ required: true })}
              id="description"
              value={organization.description}
              error={!!errors.description}
            />
            {errors.description && (
              <div className="edit-org__input-error">Description is required</div>
            )}
          </FormGroup>
          <FormGroup title="Manage Users" help="The users who are part of the organization">
            <FormGroup
              title="Add A User"
              help="Add a new user to be part of the organization"
              type={FormGroup.Type.SUBFORM}
            >
              <WepInput
                name="user"
                id="user"
                value={newUser}
                placeholder="Username..."
                handleChange={ev => setNewUser(ev.target.value)}
              />
              <Button size={Button.Size.SMALL} onClick={addUser}>
                Add User
              </Button>
            </FormGroup>
            <FormGroup
              title="Current Users"
              help="The users that are currently part of the organization"
              type={FormGroup.Type.SUBFORM}
            >
              {orgUsers.map(u => getUserDisplay(u))}
            </FormGroup>
          </FormGroup>
          {organization.id && (
            <Button onClick={deleteOrg} type={ButtonType.DANGER}>
              Delete
            </Button>
          )}
          <Button onClick={callback}>Return</Button>
          <Button
            type={ButtonType.SUCCESS}
            htmlType="submit"
            disabled={!isEmpty(errors)}
            tooltip={!isEmpty(errors) && 'There are errors in your form'}
            tooltipType="error"
          >
            Save
          </Button>
        </form>
      </div>
    </BodyCard>
  );
}

EditOrganization.propTypes = {
  organization: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object, PropTypes.array])
  ),
  currentUser: PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.bool])),
  users: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)),
  returnCallback: PropTypes.func,
};

export default EditOrganization;
