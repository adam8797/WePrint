import React, { useState }  from 'react';
import { isEmpty } from 'lodash';
import { useForm } from 'react-hook-form';
import { Link } from 'react-router-dom';
import {
  BodyCard,
  SectionTitle,
  WepInput,
  Button
} from '../../components';
import authApi from '../../api/AuthApi';
import './login.scss';

function Login () {
  const { register, handleSubmit, errors } = useForm();
  const [loginFailed, setLoginFailed] = useState(false);
  const [shouldRedirect, setShouldRedirect] = useState(false);

  const handleSubmission = form => {
    setLoginFailed(false);
    const {
      username,
      password
    } = form;

    if(!username || !password)
      return;

      authApi.login(username, password, true).subscribe(
      () => {
        setLoginFailed(false);  
        setShouldRedirect(true);
      },
      () => {
        setLoginFailed(true);
      }
    );
  };

  if(shouldRedirect) {
    window.location.href = `${window.location.origin}/`;
  }

  return (<BodyCard className='login-body' centered>
    <form onSubmit={handleSubmit(handleSubmission)}>
      <SectionTitle title="Login" />
      <div className="input-group">
        <label htmlFor="username">Username</label>
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
        <label htmlFor="password">Password</label>
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
      <div className={loginFailed ? '' : 'hide'}>Login Failed!</div>
      <div className="body-card__actions">
          <Button
            type={Button.Type.PRIMARY}
            htmlType="submit"
            size={Button.Size.LARGE}
            className="body-card__action-right"
            disabled={!isEmpty(errors)}
          >
            Login
          </Button>
        </div>
        <p>Don&apos;t have an account? <Link to='/register'>Create One!</Link></p>
    </form>
  </BodyCard>);
}
export default Login;
