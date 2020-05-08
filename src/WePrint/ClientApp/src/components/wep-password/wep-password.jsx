import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-password.scss';

function WepPassword(props) {
  const { register, name, id, placeholder = '', error } = props;
  const className = classNames('wep-password', { 'wep-password--error': error });
  return (
    <input
      ref={register}
      className={className}
      type="password"
      name={name}
      id={id}
      placeholder={placeholder}
    />
  );
}

WepPassword.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  placeholder: PropTypes.string,
  error: PropTypes.bool,
  register: PropTypes.func,
};

export default WepPassword;
