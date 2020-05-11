import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-input.scss';

function WepInput(props) {
  const { register, name, id, value, placeholder = '', handleChange, error, isPassword } = props;
  const className = classNames('wep-input', { 'wep-input--error': error });
  // if handleChange is provided, that means the parent handles onChange manually
  if (handleChange) {
    return (
      <input
        className={className}
        type={isPassword ? "password" : "text"}
        name={name}
        id={id}
        value={value}
        placeholder={placeholder}
        onChange={handleChange}
      />
    );
  }
  // otherwise we register it and let the form handle it
  return (
    <input
      ref={register}
      className={className}
      type={isPassword ? "password" : "text"}
      name={name}
      id={id}
      defaultValue={value}
      placeholder={placeholder}
    />
  );
}

WepInput.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.string,
  placeholder: PropTypes.string,
  handleChange: PropTypes.func,
  error: PropTypes.bool,
  register: PropTypes.func,
  isPassword: PropTypes.bool
};

export default WepInput;
