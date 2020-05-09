import React, { Component } from 'react';

import UserApi from '../../api/UserApi';
import { BodyCard, StatusView, AccountRestrictedView } from '../../components';
import Organization from '../organization/organization';

import './my-org.scss';

class MyOrg extends Component {
  constructor(props) {
    super(props);
    this.state = {
      error: false,
      user: null,
    };
  }

  componentDidMount() {
    UserApi.CurrentUser().subscribe(
      u => {
        this.setState({ user: u });
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  render() {
    const { error, user } = this.state;
    if (error) {
      return (
        <BodyCard>
          <StatusView text="Could not fetch current user" icon={['far', 'frown']} />
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

    if (!user) {
      return (
        <BodyCard>
          <AccountRestrictedView text="Only logged in users can have organizations" />
        </BodyCard>
      );
    }

    if (user.organization) {
      return <Organization user={user} orgId={user.organization} />;
    }

    return (
      <BodyCard>
        <div className="my-org">
          <div className="my-org__text">You do not have an organization.</div>
          <div className="my-org__sub-text">
            <a href="/new-organization">Click here</a> to create one, or contact your supervisor to
            have one set up.
          </div>
        </div>
      </BodyCard>
    );
  }
}

export default MyOrg;
