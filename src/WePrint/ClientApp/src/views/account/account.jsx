import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import UserApi from '../../api/UserApi';
import { BodyCard, StatusView, AccountRestrictedView } from '../../components';
import EditAccount from './edit-account';

class Account extends Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null,
      error: false,
      loggedIn: true,
    };
  }

  componentDidMount() {
    UserApi.CurrentUser().subscribe(
      u => {
        this.setState({ user: u });
      },
      err => {
        if (err.response.status === 401) {
          this.setState({ loggedIn: false });
          return;
        }
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  render() {
    const { error, user, loggedIn } = this.state;
    if (error) {
      return (
        <BodyCard>
          <StatusView text="Could not load your user information" icon={['far', 'frown']} />
        </BodyCard>
      );
    }

    if (!loggedIn) {
      return (
        <BodyCard>
          <AccountRestrictedView />
        </BodyCard>
      );
    }

    if (user === null) {
      return (
        <BodyCard>
          <StatusView text="Loading..." icon="sync" spin />
        </BodyCard>
      );
    }
    return <EditAccount currentUser={user} />;
  }
}

export default withRouter(Account);
