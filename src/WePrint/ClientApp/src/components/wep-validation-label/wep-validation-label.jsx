import React from 'react';
import PropTypes from 'prop-types';
import './wep-validation-label.scss';

function WepValidationLabel(props) {
  const { forItem, caption, error } = props;
  return <>
    <label htmlFor={forItem}>{caption}</label>
    { error ? <p className='wep-validation-label-error'>{error}</p> : <></>}
  </>
}

WepValidationLabel.propTypes = {
  forItem: PropTypes.string,
  caption: PropTypes.string,
  error: PropTypes.string
};

export default WepValidationLabel;
