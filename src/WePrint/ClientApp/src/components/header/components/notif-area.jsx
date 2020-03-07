import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import Jdenticon from 'react-jdenticon';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import UserApi from '../../../api/UserApi';
import Button from '../../button/button';
import './notif-area.scss';

class NotifArea extends Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null,
    };
  }

  componentDidMount() {
    UserApi.CurrentUser().subscribe(user => {
      this.setState({ user });
    });
  }

  logIn = e => {
    //this.props.history.push('route to login screen')
    e.preventDefault();
  };

  render() {
    const { user } = this.state;
    return (
      <div className="notif-area">
        {user ? (
          <div className="notif-area__userInfo">
            <div className="notif-area__name">
              {user.firstName || user.lastName
                ? `${user.firstName} ${user.lastName}`
                : 'Unnamed User'}
            </div>
            <div className="notif-area__avatar">
              {user.avatar ? (
                <img src={user.avatar} alt="User Avatar" />
              ) : (
                <Jdenticon value={`${user.id}`} />
              )}
            </div>
            <div className="notif-area__icon">
              <FontAwesomeIcon icon="bars" />
            </div>
          </div>
        ) : (
          <div className="notif-area__log-in">
            <span>You are not logged in</span>
            <Button size={Button.Size.SMALL} onClick={this.logIn}>
              Log in!
            </Button>
          </div>
        )}
      </div>
    );
  }
}

export default withRouter(NotifArea);
