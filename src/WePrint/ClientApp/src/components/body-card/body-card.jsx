import React from 'react';
import PropTypes from 'prop-types';
import classNames from 'classnames';
import './body-card.scss';

function BodyCard(props) {
  const { centered, className = '', children } = props;
  const cardClass = classNames('body-card', className, { 'body-card--centered': centered });

  return <div className={cardClass}>{children}</div>;
}

BodyCard.propTypes = {
  children: PropTypes.node.isRequired,
  centered: PropTypes.bool,
  className: PropTypes.string,
};

BodyCard.defaultProps = {
  centered: false,
};

export default BodyCard;
