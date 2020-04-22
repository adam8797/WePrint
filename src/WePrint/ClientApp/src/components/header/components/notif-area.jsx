import React, { Component } from 'react';
import { withRouter, Redirect } from 'react-router-dom';

import UserApi from '../../../api/UserApi';
import Button from '../../button/button';
import './notif-area.scss';

class NotifArea extends Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null,
      loaded: false,
    };
  }

  componentDidMount() {
    UserApi.CurrentUser().subscribe({
      next: user => {
        this.setState({ user, loaded: true });
      },
      error: err => {
        console.error(err);
        // Set state to loaded to show that the api call was tried even if it didn't work
        this.setState({ loaded: true });
      },
    });
  }

  logIn = () => {
    this.setState({ toLogin: true });
  };

  logOut = () => {
    this.setState({ toLogout: true });
  };

  render() {
    const { user, loaded, toLogin, toLogout } = this.state;

    if (toLogin) {
      return <Redirect to="/login" />;
    }
    if (toLogout) {
      return <Redirect to="/logout" />;
    }

    if (!loaded) {
      return <div className="notif-area" />;
    }
    return (
      <div className="notif-area">
        {user ? (
          <div className="notif-area__userInfo">
            <div className="notif-area__name">
              {user.firstName || user.lastName
                ? `${user.firstName} ${user.lastName}`
                : user.username}
            </div>
            <div className="notif-area__avatar">
              <img className="notif-area__ava-icon" src={`/api/users/by-id/${user.id}/avatar`} alt="User Avatar"/>
            </div>
            {/* <div className="notif-area__icon">
              <FontAwesomeIcon icon="bars" />
            </div> */}
            <Button size={Button.Size.SMALL} onClick={this.logOut}>
              Log out
            </Button>
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
