import React from 'react';
import PropTypes from 'prop-types';
import './section-title.scss';

function SectionTitle({ title, id }) {
  // create anchors for sections to link directly to them
  const anchor = id || `${title.toLocaleLowerCase()}-section`;
  return (
    <div className="section-title" id={anchor}>
      <h2>{title}</h2>
    </div>
  );
}

SectionTitle.propTypes = {
  title: PropTypes.string.isRequired,
  id: PropTypes.string,
};

SectionTitle.defaultProps = {
  id: '',
};

export default SectionTitle;
