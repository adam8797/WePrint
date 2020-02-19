import React from 'react';
import PropTypes from 'prop-types';
import './body-card.scss';

function BodyCard(props) {
  const { children } = props;

  return <div className="body-card">{children}</div>;
}

BodyCard.propTypes = {
  children: PropTypes.node.isRequired,
};

export default BodyCard;
