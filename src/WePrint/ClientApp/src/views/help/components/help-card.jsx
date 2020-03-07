import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import './help-card.scss';

class HelpCard extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <div className="help-card" onClick={this.props.onClick}>
        <span className="help-card__text">{this.props.text}</span>
        <FontAwesomeIcon className="help-card__icon" icon={['far', this.props.icon]} size="5x" />
      </div>
    );
  }
}

HelpCard.propTypes = {
  text: PropTypes.string.isRequired,
  icon: PropTypes.string,
};

export default HelpCard;
