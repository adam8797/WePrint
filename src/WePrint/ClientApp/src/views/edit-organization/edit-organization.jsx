import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

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

function EditOrganization(props) {
  const { currentUser, users, returnCallback } = props;
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

  // basic info
  const [name, setName] = useState(organization.name);
  const [description, setDescription] = useState(organization.description);

  // address info
  const [addressLine1, setAddressLine1] = useState(organization.address.addressLine1);
  const [addressLine2, setAddressLine2] = useState(organization.address.addressLine2);
  const [addressLine3, setAddressLine3] = useState(organization.address.addressLine3);
  const [attention, setAttention] = useState(organization.address.attention);
  const [city, setCity] = useState(organization.address.city);
  const [state, setState] = useState(organization.address.state);
  const [zipCode, setZipCode] = useState(organization.address.zipCode);

  // users
  const [newUser, setNewUser] = useState(null);
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

  const saveOrg = () => {
    const org = organization;
    organization.name = name;
    organization.description = description;
    organization.address = {
      attention,
      addressLine1,
      addressLine2,
      addressLine3,
      city,
      state,
      zipCode,
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
        <div className="edit-org__split-inline edit-org__split-inline--equal">
          <div>
            <FormGroup title="Organization Name" help="What is the organization called?">
              <WepInput
                name="name"
                id="name"
                value={name}
                placeholder="Organization Name..."
                handleChange={ev => setName(ev.target.value)}
              />
            </FormGroup>
            <FormGroup title="Location" help="Where is the organization located?">
              <label htmlFor="attention">Attention</label>
              <WepInput
                name="attention"
                id="attention"
                value={attention}
                handleChange={ev => setAttention(ev.target.value)}
              />
              <label htmlFor="addressLine1">Address</label>
              <WepInput
                name="addressLine1"
                id="addressLine1"
                value={addressLine1}
                handleChange={ev => setAddressLine1(ev.target.value)}
              />
              <WepInput
                name="addressLine2"
                id="addressLine2"
                value={addressLine2}
                handleChange={ev => setAddressLine2(ev.target.value)}
              />
              <WepInput
                name="addressLine3"
                id="addressLine3"
                value={addressLine3}
                handleChange={ev => setAddressLine3(ev.target.value)}
              />
              <label htmlFor="city">City</label>
              <WepInput
                name="city"
                id="city"
                value={city}
                handleChange={ev => setCity(ev.target.value)}
              />
              <label htmlFor="state">State</label>
              <WepInput
                name="state"
                id="state"
                value={state}
                handleChange={ev => setState(ev.target.value)}
              />
              <label htmlFor="zipCode">Zip</label>
              <WepInput
                name="zipCode"
                id="zipCode"
                value={zipCode}
                handleChange={ev => setZipCode(ev.target.value)}
              />
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
                )) || <div className="edit-org__logo edit-org__logo--blank">No Logo Uploaded</div>}
                <FileDrop
                  handleFiles={uploadFile}
                  accept=".png, .jpg, .jpeg"
                  customMsg="Drag an image file here, or click to select one"
                />
              </div>
            </FormGroup>
          </div>
        </div>
        <FormGroup title="Organization Bio" help="A description of the organization">
          <WepTextarea
            name="description"
            id="description"
            value={description}
            handleChange={ev => setDescription(ev.target.value)}
          />
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
        <Button onClick={saveOrg} type={ButtonType.SUCCESS}>
          Save
        </Button>
      </div>
    </BodyCard>
  );
}

EditOrganization.propTypes = {
  organization: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object])
  ),
  currentUser: PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.bool])),
  users: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)),
  returnCallback: PropTypes.func,
};

export default EditOrganization;
