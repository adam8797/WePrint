import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-number.scss';

const noop = () => {};

function WepNumber(props) {
  const { name, id, value, min, max, step, placeholder = '', handleChange = noop, error } = props;
  const className = classNames('wep-number', { 'wep-number--error': error });
  return (
    <input
      className={className}
      type="number"
      name={name}
      id={id}
      value={value}
      min={min}
      max={max}
      step={step}
      placeholder={placeholder}
      onChange={handleChange}
    />
  );
}

WepNumber.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.string,
  min: PropTypes.number,
  max: PropTypes.number,
  step: PropTypes.number,
  placeholder: PropTypes.string,
  handleChange: PropTypes.func,
  error: PropTypes.bool,
};

export default WepNumber;
