import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-input.scss';

const noop = () => {};

function WepInput(props) {
  const { name, id, value, placeholder = '', handleChange = noop, error } = props;
  const className = classNames('wep-input', { 'wep-input--error': error });
  return (
    <input
      className={className}
      type="text"
      name={name}
      id={id}
      value={value}
      placeholder={placeholder}
      onChange={handleChange}
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
};

export default WepInput;
