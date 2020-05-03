import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-textarea.scss';

const noop = () => {};

function WepTextarea(props) {
  const { name, id, value, placeholder = '', handleChange = noop, error, register } = props;
  const className = classNames('wep-textarea', { 'wep-textarea--error': error });
  return (
    <div className="wep-textarea__wrapper">
      <textarea
        ref={register}
        className={className}
        name={name}
        id={id}
        value={value}
        placeholder={placeholder}
        onChange={handleChange}
      />
    </div>
  );
}

WepTextarea.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.string,
  placeholder: PropTypes.string,
  handleChange: PropTypes.func,
  error: PropTypes.bool,
  register: PropTypes.func,
};

export default WepTextarea;
