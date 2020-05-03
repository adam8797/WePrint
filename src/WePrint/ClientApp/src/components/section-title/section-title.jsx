import React from 'react';
import PropTypes from 'prop-types';
import Button from '../button/button';
import './section-title.scss';

function SectionTitle({ title, id, actions, headerSize }) {
  // create anchors for sections to link directly to them
  const anchor = id || `${title.toLocaleLowerCase()}-section`;

  const HeaderTag = headerSize === undefined ? "h2" : `h${headerSize}`;

  const actionItems = actions.map(action => (
    <Button
      size={Button.Size.SMALL}
      type={Button.Type.PRIMARY}
      key={action.key}
      onClick={action.action}
      icon={action.icon}
      selected={action.selected}
    >
      {action.text}
    </Button>
  ));

  return (
    <div className="section-title" id={anchor}>
      <HeaderTag>{title}</HeaderTag>
      <div className="section-title__actions">{actionItems}</div>
    </div>
  );
}

SectionTitle.propTypes = {
  title: PropTypes.string.isRequired,
  id: PropTypes.string,
  actions: PropTypes.arrayOf(
    PropTypes.shape({
      text: PropTypes.string,
      key: PropTypes.string,
      action: PropTypes.func,
      icon: PropTypes.string,
    })
  ),
  headerSize: PropTypes.number,
};

SectionTitle.defaultProps = {
  id: '',
  actions: [],
};

export default SectionTitle;
