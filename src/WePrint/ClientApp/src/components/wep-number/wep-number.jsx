import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-number.scss';

function WepNumber(props) {
  const {
    register,
    name,
    id,
    value,
    min,
    max,
    step,
    placeholder = '',
    handleChange,
    error,
  } = props;
  const className = classNames('wep-number', { 'wep-number--error': error });
  if (handleChange) {
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

  return (
    <input
      ref={register}
      className={className}
      type="number"
      name={name}
      id={id}
      defaultValue={value}
      min={min}
      max={max}
      step={step}
      placeholder={placeholder}
    />
  );
}

WepNumber.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  min: PropTypes.number,
  max: PropTypes.number,
  step: PropTypes.number,
  placeholder: PropTypes.string,
  handleChange: PropTypes.func,
  error: PropTypes.bool,
  register: PropTypes.func,
};

export default WepNumber;
