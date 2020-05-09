import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useForm } from 'react-hook-form';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import UserApi from '../../api/UserApi';
import AuthApi from '../../api/AuthApi';

import {
  BodyCard,
  WepInput,
  WepPassword,
  WepTextarea,
  FileDrop,
  Button,
  toastMessage,
  toastError,
  FormGroup,
  SectionTitle,
  WepModal,
} from '../../components';

import './edit-account.scss';

function EditAccount(props) {
  const { currentUser } = props;

  const { register: userRegister, errors: userErrors, handleSubmit: handleUserSubmit } = useForm();
  const {
    register: passRegister,
    errors: passErrors,
    handleSubmit: handlePassSubmit,
    watch,
  } = useForm();

  const [logoUrl, setLogoUrl] = useState(UserApi.getAvatarUrl(currentUser.id));
  const [modalOpen, setModalOpen] = useState(false);

  if (!currentUser) {
    return (
      <BodyCard>
        <div className="user__error">
          <span className="user__error-text">Could not load your user information</span>
          <FontAwesomeIcon icon={['far', 'frown']} />
        </div>
      </BodyCard>
    );
  }

  const saveUser = form => {
    const payload = currentUser;
    payload.firstName = form.firstName;
    payload.lastName = form.lastName;
    payload.bio = form.bio;
    UserApi.UpdateUser(payload).subscribe(
      () => toastMessage('Account successfully updated'),
      err => {
        console.error(err);
        toastError('Could not update your account');
      }
    );
  };

  const deleteUser = () => {
    // we may want to do this in the future
  };

  const changePassword = form => {
    const { oldPassword, newPassword } = form;
    const payload = {
      oldPassword,
      newPassword,
    };
    AuthApi.changePassword(payload).subscribe(
      () => toastMessage('Password successfully updated'),
      err => {
        console.error(err);
        toastError('Could not update password');
      }
    );
  };

  const uploadFile = files => {
    const data = new FormData();
    data.append('postedImage', files[0]);
    UserApi.postAvatar(data).subscribe(
      () => {
        const reader = new FileReader();
        reader.onload = () => {
          setLogoUrl(reader.result);
          toastMessage('Avatar successfully updated');
        };
        reader.readAsDataURL(files[0]);
      },
      err => {
        console.error(err);
        toastError('Could not upload avatar');
      }
    );
  };

  return (
    <BodyCard>
      <div className="edit-acct">
        <SectionTitle title="User Information" />
        <form onSubmit={handleUserSubmit(saveUser)} className="edit-acct__basic-info">
          <div className="edit-acct__fields">
            <FormGroup title="Name">
              <FormGroup
                title="First Name"
                type={FormGroup.Type.SUBFORM}
                styles={[FormGroup.Style.CONDENSED]}
              >
                <WepInput
                  name="firstName"
                  register={userRegister({ required: true })}
                  id="firstName"
                  label="First Name"
                  placeholder="First name"
                  value={currentUser.firstName}
                  error={!!userErrors.firstName}
                />
                {userErrors.firstName && (
                  <div className="edit-acct__input-error">First name is required</div>
                )}
              </FormGroup>
              <FormGroup title="Last Name" type={FormGroup.Type.SUBFORM}>
                <WepInput
                  name="lastName"
                  register={userRegister({ required: true })}
                  id="lastName"
                  label="Last Name"
                  placeholder="Last Name"
                  value={currentUser.lastName}
                  error={!!userErrors.lastName}
                />
                {userErrors.lastName && (
                  <div className="edit-acct__input-error">Last name is required</div>
                )}
              </FormGroup>
            </FormGroup>
            <FormGroup title="Username" help="This is what you will use to log in">
              <WepInput
                name="username"
                register={userRegister({ required: true })}
                id="username"
                label="Username"
                placeholder="Username"
                value={currentUser.username}
                error={!!userErrors.username}
              />
              {userErrors.username && (
                <div className="edit-acct__input-error">Username is required</div>
              )}
            </FormGroup>
            <FormGroup title="Bio">
              <WepTextarea
                name="bio"
                register={userRegister}
                id="bio"
                label="Bio"
                placeholder="Bio"
                value={currentUser.bio}
              />
            </FormGroup>
            <div className="edit-acct__button-container">
              <Button type={Button.Type.SUCCESS} htmlType="submit">
                Save
              </Button>
              {false && (
                <Button type={Button.Type.DANGER} onClick={() => setModalOpen(true)}>
                  Delete Account
                </Button>
              )}
            </div>
          </div>
          <div className="edit-acct__prof-pic">
            <FormGroup title="Avatar">
              <div className="edit-acct__logo-container">
                <img className="edit-acct__logo" src={logoUrl} alt="User Avatar" />
                <FileDrop
                  handleFiles={uploadFile}
                  name="logo"
                  accept=".png, .jpg, .jpeg"
                  customMsg="Drag an image file here, or click to select one"
                />
              </div>
            </FormGroup>
          </div>
        </form>
        <hr />
        <SectionTitle title="Change Password" />
        <form onSubmit={handlePassSubmit(changePassword)} className="edit-acct__password">
          <FormGroup title="Current Password">
            <WepPassword
              name="oldPassword"
              register={passRegister({ required: true })}
              id="oldPassword"
              label="Current Password"
              placeholder="Current Password"
              error={!!passErrors.oldPassword}
            />
            {passErrors.oldPassword && (
              <div className="edit-acct__input-error">Current Password is required</div>
            )}
          </FormGroup>
          <FormGroup title="New Password">
            <WepPassword
              name="newPassword"
              register={passRegister({ required: true })}
              id="newPassword"
              label="New Password"
              placeholder="New Password"
              error={!!passErrors.newPassword}
            />
            {passErrors.newPassword && (
              <div className="edit-acct__input-error">New Password is required</div>
            )}
          </FormGroup>
          <FormGroup title="Confirm New Password">
            <WepPassword
              name="confirmPass"
              register={passRegister({
                required: true,
                validate: value => {
                  return value === watch('newPassword');
                },
              })}
              id="confirmPass"
              label="Confirm New Password"
              placeholder="New Password"
              error={!!passErrors.confirmPass}
            />
            {passErrors.confirmPass && (
              <div className="edit-acct__input-error">Passwords must match</div>
            )}
          </FormGroup>
          <Button type={Button.Type.SUCCESS} htmlType="submit">
            Save
          </Button>
        </form>
        <WepModal
          isOpen={modalOpen}
          onRequestClose={() => setModalOpen(false)}
          contentLabel="Delete Account"
        >
          <SectionTitle title="Delete Account" />
          <div className="edit-acct__modal-text">
            Are you sure you want to delete your account?
            <br />
            This action is irreversible
          </div>
          <div className="edit-acct__button-container">
            <Button type={Button.Type.DANGER} onClick={deleteUser}>
              Delete My Account
            </Button>
            <Button onClick={() => setModalOpen(false)}>Cancel</Button>
          </div>
        </WepModal>
      </div>
    </BodyCard>
  );
}

EditAccount.propTypes = {
  currentUser: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object])
  ),
};

export default EditAccount;
