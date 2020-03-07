import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './wep-dropdown.scss';

const noop = () => {};

function WepDropdown(props) {
  const { name, id, value, options, error, placeholder = '', handleChange = noop } = props;

  const className = classNames('wep-dropdown', { 'wep-dropdown--error': error });

  return (
    <select
      name={name}
      id={id}
      value={value}
      className={className}
      placeholder={placeholder}
      onChange={handleChange}
    >
      {options &&
        options.map(opt => (
          <option value={opt.value} key={opt.value}>
            {opt.displayName}
          </option>
        ))}
    </select>
  );
}

WepDropdown.propTypes = {
  name: PropTypes.string,
  id: PropTypes.string,
  value: PropTypes.string,
  placeholder: PropTypes.string,
  error: PropTypes.bool,
  options: PropTypes.arrayOf(
    PropTypes.shape({
      value: PropTypes.string,
      displayName: PropTypes.string,
    })
  ),
  handleChange: PropTypes.func,
};

export default WepDropdown;
