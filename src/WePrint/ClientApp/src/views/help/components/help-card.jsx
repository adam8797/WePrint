import React from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import './help-card.scss';

function HelpCard(props) {
  const { onClick, text, icon } = props;
  return (
    <div className="help-card" onClick={onClick} onKeyDown={onClick}>
      <span className="help-card__text">{text}</span>
      <FontAwesomeIcon className="help-card__icon" icon={['far', icon]} size="5x" />
    </div>
  );
}

HelpCard.propTypes = {
  text: PropTypes.string.isRequired,
  icon: PropTypes.string,
  onClick: PropTypes.func,
};

export default HelpCard;
