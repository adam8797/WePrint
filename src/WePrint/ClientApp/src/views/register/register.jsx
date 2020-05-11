import React, { useState }  from 'react';
import { isEmpty } from 'lodash';
import { useForm } from 'react-hook-form';
import {
  BodyCard,
  SectionTitle,
  WepInput,
  Button,
  WepValidationLabel
} from '../../components';
import authApi from '../../api/AuthApi';
import './register.scss';

function validateForm(values){
  const errors = {};
  const {  password, confirmPassword } = values;
  if(!/\d/.test(password))
    errors.password = 'Password must contain a number';
  if(!/[^\da-zA-Z]/.test(password))
    errors.password = 'Password must contain a symbol';
  if(password.length < 6)
    errors.password = 'Password must be at least 6 characters';
  if(confirmPassword !== password){
    errors.confirmPassword = 'Passwords Don\'t Match';
  }
  return { values, errors };
}

function Register () {
  const { register, handleSubmit, errors } = useForm({ mode: 'onChange', validationResolver: validateForm });
  const [registerFailed, setRegisterFailed] = useState(false);
  const [generalErrors, setGeneralErrors] = useState('');
  const [shouldRedirect, setShouldRedirect] = useState(false);

  const handleSubmission = form => {
    const {
      email,
      username,
      password,
      firstName,
      lastName
    } = form;

    if(!username || !password)
      return;

      authApi.register(email, username, firstName, lastName, password).subscribe(
      () => {
        setRegisterFailed(false);  
        setShouldRedirect(true);
      },
      err => {
        for(let i = 0; i < err.response.data.length; i++) {
          if(err.response.data[i].code === 'DuplicateUserName') {
            setGeneralErrors('Username is taken');
          }
        }
        setRegisterFailed(true);
      }
    );
  };

  if(shouldRedirect) {
    window.location.href = `${window.location.origin}/`;
  }

  return (<BodyCard className='register-body' centered>
    <form onSubmit={handleSubmit(handleSubmission)}>
      <SectionTitle title="Create Account" />
      <div className="input-group">
        <WepValidationLabel caption='Email Address' forItem='email' error={errors.email} />
        <WepInput
          name="email"
          register={register(
            { 
              required: true, 
              pattern: {
                // Not my regex, might need to review this
                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i,
                message: "invalid email address"
              } 
            })}
          id="email"
          value=""
          placeholder=""
          error={!!errors.email}
        />
      </div>
      <div className="input-group">
        <WepValidationLabel caption='Username' forItem='username' error={errors.username} />
        <WepInput
          name="username"
          register={register({ required: true })}
          id="username"
          value=""
          placeholder=""
          error={!!errors.username}
        />
      </div>
      <div className="input-group">
        <WepValidationLabel caption='Real Name' forItem='firstName' error={errors.firstName || errors.lastName} />
        <WepInput
          name="firstName"
          register={register({ required: true })}
          id="firstName"
          value=""
          placeholder="First Name"
          error={!!errors.firstName}
        />
        <WepInput
          name="lastName"
          register={register({ required: true })}
          id="lastName"
          value=""
          placeholder="Last Name"
          error={!!errors.lastName}
        />
      </div>
      <div className="input-group">
        <WepValidationLabel caption='Password' forItem='password' error={errors.password} />
        <WepInput
          name="password"
          register={register({ required: true })}
          id="password"
          value=""
          placeholder=""
          error={!!errors.password}
          isPassword
        />
      </div>
      <div className="input-group">
        <WepValidationLabel caption='Confirm Password' forItem='confirmPassword' error={errors.confirmPassword} />
        <WepInput
          name="confirmPassword"
          register={register({ required: true })}
          id="confirmPassword"
          value=""
          placeholder=""
          error={!!errors.confirmPassword}
          isPassword
        />
      </div>
          <div className={registerFailed ? '' : 'hide'}>Registration Failed! <span className='general-error'>{generalErrors}</span></div>
      <div className="body-card__actions">
          <Button
            type={Button.Type.PRIMARY}
            htmlType="submit"
            size={Button.Size.LARGE}
            className="body-card__action-right"
            disabled={!isEmpty(errors)}
          >
            Create Account
          </Button>
        </div>
    </form>
  </BodyCard>);
}
export default Register;
