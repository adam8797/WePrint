import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './body-card.scss';

function BodyCard(props) {
  const { centered, children } = props;
  const className = classNames('body-card', { 'body-card--centered': centered });

  return <div className={className}>{children}</div>;
}

BodyCard.propTypes = {
  children: PropTypes.node.isRequired,
  centered: PropTypes.bool,
};

BodyCard.defaultProps = {
  centered: false,
};

export default BodyCard;
