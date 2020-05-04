import React from 'react';
import PropTypes from 'prop-types';
import Modal from 'react-modal';
import classNames from 'classnames';
import './wep-modal.scss';

// Josh: I'm not entirely sure this is the best way to do this but I wanted some control over
// this class-named element for consistent, reusable styling.
function WepModalButtons(props) {
  const { children } = props;
  return <div className="wep-modal__button-container">{children}</div>;
}

WepModalButtons.propTypes = {
  children: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
};

function WepModal(props) {
  const { children, className, isOpen, onRequestClose, contentLabel, ...rest } = props;

  const modalClass = classNames('wep-modal', className);

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onRequestClose}
      contentLabel={contentLabel}
      className={modalClass}
      overlayClassName="wep-modal__overlay"
      {...rest}
    >
      {children}
    </Modal>
  );
}

WepModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onRequestClose: PropTypes.func.isRequired,
  contentLabel: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
};

WepModal.ButtonContainer = WepModalButtons;

export default WepModal;
