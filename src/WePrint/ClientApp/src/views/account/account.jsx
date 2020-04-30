import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';

import UserApi from '../../api/UserApi';
import { BodyCard, StatusView } from '../../components';
import EditAccount from './edit-account';

class Account extends Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null,
      error: false,
    };
    UserApi.CurrentUser().subscribe(user => {
      this.setState({ user });
    });
  }

  render() {
    const { error, user } = this.state;
    if (error || user == null) {
      return (
        <BodyCard>
          <StatusView text="Could not load your user information" icon={['far', 'frown']} />
        </BodyCard>
      );
    }
    return <EditAccount currentUser={user} />;
  }
}

export default withRouter(Account);
