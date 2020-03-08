import React, { Component } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { find } from 'lodash';
import HelpCard from './components/help-card';
import BodyCard from '../../components/body-card/body-card';
import SectionTitle from '../../components/section-title/section-title';
import './help.scss';

class Help extends Component {
  constructor(props) {
    super(props);
    this.state = {
      detail: null,
      title: null,
    };
    this.cards = [
      { text: 'Questions', icon: 'question-circle', detail: 'questions' },
      { text: 'Contact Us', icon: 'comments', detail: 'contact' },
      { text: 'About Us', icon: 'building', detail: 'about' },
    ];
  }

  setDetailView = detail => {
    const title = detail ? find(this.cards, { detail: detail }).text : null;
    this.setState({ detail, title });
  };

  getContents = detail => {
    switch (detail) {
      case 'questions':
        return (
          <div className="help__content">
            <p>
              If you have questions, please contact us directly via our{' '}
              <span className="help__link" onClick={() => this.setDetailView('contact')}>
                Contact Us
              </span>{' '}
              page!
            </p>
          </div>
        );
      case 'contact':
        return (
          <div className="help__content">
            Email us at <a href="mailto:help@weprint.io">help@weprint.io</a> with any questions or
            concerns!
          </div>
        );
      case 'about':
        return (
          <div className="help__content">
            WePrint is a service which connects people looking to get 3D prints made with those who
            make prints!
          </div>
        );
      default:
        return <div></div>;
    }
  };

  render() {
    const { detail, title } = this.state;
    return (
      <div className="help">
        {detail && (
          <BodyCard>
            <div className="help__link" onClick={() => this.setDetailView(null)}>
              <FontAwesomeIcon icon="arrow-left" /> Back
            </div>
            <SectionTitle title={title} />
            {this.getContents(detail)}
          </BodyCard>
        )}
        {!detail && (
          <div className="help__selection">
            <h1 className="help__selection-text">How Can We Help You?</h1>
            <div className="help__selection-cards">
              {this.cards.map(card => {
                return (
                  <HelpCard
                    key={card.detail}
                    text={card.text}
                    icon={card.icon}
                    onClick={() => {
                      this.setDetailView(card.detail);
                    }}
                  />
                );
              })}
            </div>
          </div>
        )}
      </div>
    );
  }
}

export default Help;
