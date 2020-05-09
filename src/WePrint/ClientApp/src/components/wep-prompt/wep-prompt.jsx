/* eslint-disable react/no-array-index-key */
import React from 'react';
import PropTypes from 'prop-types';
import { Prompt } from 'react-router-dom';
import Modal from 'react-modal';
import Button from '../button/button';
import WepModal from '../wep-modal/wep-modal';
import './wep-prompt.scss';

Modal.setAppElement('#root');

export class WepPrompt extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      modalOpen: false,
      lastLocation: null,
      confirmedNavigation: false,
    };
  }

  showModal = location =>
    this.setState({
      modalOpen: true,
      lastLocation: location,
    });

  closeModal = callback => {
    this.setState(
      {
        modalOpen: false,
      },
      callback
    );
  };

  handleBlockedNavigation = nextLocation => {
    const { confirmedNavigation } = this.state;
    if (!confirmedNavigation) {
      this.showModal(nextLocation);
      return false;
    }

    return true;
  };

  handleConfirmNavigationClick = () =>
    this.closeModal(() => {
      const { navigate } = this.props;
      const { lastLocation } = this.state;
      if (lastLocation) {
        this.setState(
          {
            confirmedNavigation: true,
          },
          () => {
            // Navigate to the previous blocked location with your navigate function
            navigate(lastLocation.pathname);
          }
        );
      }
    });

  render() {
    const { when, messages } = this.props;
    const { modalOpen } = this.state;
    return (
      <>
        <Prompt when={when} message={this.handleBlockedNavigation} />
        <WepModal
          isOpen={modalOpen}
          onRequestClose={() => this.closeModal()}
          contentLabel="Confirm Navigation"
        >
          <h3>Are you sure?</h3>
          <div className="wep-prompt__message-container">
            {messages.map((message, i) => (
              <div key={i}>{message}</div>
            ))}
          </div>
          <WepModal.ButtonContainer>
            <Button
              onClick={() => this.closeModal()}
              type={Button.Type.DANGER}
              className="wep-prompt__button"
            >
              Cancel
            </Button>
            <Button
              onClick={this.handleConfirmNavigationClick}
              type={Button.Type.SUCCESS}
              className="wep-prompt__button"
            >
              Confirm
            </Button>
          </WepModal.ButtonContainer>
        </WepModal>
      </>
    );
  }
}

WepPrompt.propTypes = {
  when: PropTypes.bool.isRequired,
  messages: PropTypes.arrayOf(PropTypes.string).isRequired,
  navigate: PropTypes.func.isRequired,
};

export default WepPrompt;
