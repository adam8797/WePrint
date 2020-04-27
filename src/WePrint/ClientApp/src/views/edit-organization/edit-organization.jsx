import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import UserApi from '../../api/UserApi';
import OrgApi from '../../api/OrganizationApi';

import FormGroup from '../../components/form-group/form-group';
import BodyCard from '../../components/body-card/body-card';
import WepInput from '../../components/wep-input/wep-input';
import WepTextarea from '../../components/wep-textarea/wep-textarea';
import Button, { ButtonType } from '../../components/button/button';

import './edit-organization.scss';

function EditOrganization(props) {
  const { currentUser, users, returnCallback } = props;
  let { organization } = props;
  if (!organization) {
    organization = { name: 'New Organization', address: {} };
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
    let org = organization;
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
    if (organization.id) {
      OrgApi.replace(organization.id, org).subscribe(returnCallback, console.error);
    } else {
      OrgApi.create(org).subscribe(returnCallback, console.error);
    }
  };

  const deleteOrg = () => {
    if (!organization.id) return;
    OrgApi.delete(organization.id).subscribe(returnCallback, console.error);
  };

  const getUserDisplay = u => {
    if (!u) return;
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
    return <BodyCard>You already have an organization!</BodyCard>;
  }
  return (
    <BodyCard>
      <div className="edit-org">
        <div className="edit-org__header">{organization.name}</div>
        <hr />
        <div className="edit-org__split-inline">
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
              Attention
              <WepInput
                name="attention"
                id="attention"
                value={attention}
                handleChange={ev => setAttention(ev.target.value)}
              />
              Address
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
              City
              <WepInput
                name="city"
                id="city"
                value={city}
                handleChange={ev => setCity(ev.target.value)}
              />
              State
              <WepInput
                name="state"
                id="state"
                value={state}
                handleChange={ev => setState(ev.target.value)}
              />
              Zip
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
            ></FormGroup>
          </div>
        </div>
        <FormGroup title="Organization Bio" help="A description of the organization">
          <WepTextarea
            name="description"
            id="description"
            value={description}
            handleChange={ev => setDescription(ev.target.value)}
          ></WepTextarea>
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
        <Button onClick={returnCallback}>Return</Button>
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
  currentUser: PropTypes.objectOf(PropTypes.string),
  users: PropTypes.arrayOf(PropTypes.objectOf(PropTypes.string)),
  returnCallback: PropTypes.func,
};

export default EditOrganization;
