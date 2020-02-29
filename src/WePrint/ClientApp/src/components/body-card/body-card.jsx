import React from 'react';
import PropTypes from 'prop-types';
import './body-card.scss';

function BodyCard(props) {
  const { centered, children } = props;

  return <div className={`body-card${centered ? ' body-card--centered' : ''}`}>{children}</div>;
}

BodyCard.propTypes = {
  children: PropTypes.node.isRequired,
  centered: PropTypes.bool,
};

BodyCard.defaultProps = {
  centered: false,
};

export default BodyCard;
