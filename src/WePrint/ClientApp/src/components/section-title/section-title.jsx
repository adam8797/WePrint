import React from 'react';
import PropTypes from 'prop-types';
import { Button } from '../../components';
import './section-title.scss';

function SectionTitle({ title, id, actions }) {
  // create anchors for sections to link directly to them
  const anchor = id || `${title.toLocaleLowerCase()}-section`;

  const actionItems = actions.map(action => (
    <Button type={Button.Type.PRIMARY} key={action.key} onClick={action.action}>
      {action.text}
    </Button>
  ));

  return (
    <div className="section-title" id={anchor}>
      <h2>{title}</h2>
      <div>{actionItems}</div>
    </div>
  );
}

SectionTitle.propTypes = {
  title: PropTypes.string.isRequired,
  id: PropTypes.string,
  actions: PropTypes.arrayOf(
    PropTypes.shape({ text: PropTypes.string, key: PropTypes.string, action: PropTypes.func })
  ),
};

SectionTitle.defaultProps = {
  id: '',
  actions: [],
};

export default SectionTitle;
