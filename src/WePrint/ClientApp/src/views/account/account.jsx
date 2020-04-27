import React, { Component } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { withRouter } from 'react-router-dom';

import UserApi from '../../api/UserApi';
import { BodyCard } from '../../components';
import EditAccount from './edit-account';


class Account extends Component {
    constructor(props) {
        super(props);
        this.state = {
            user: null,
            error: false
        }
        UserApi.CurrentUser().subscribe(user => {
            this.setState({ user });
        });
    }

    render() {
        const { error, user } = this.state;
        if (error || user == null) {
            return (
                <BodyCard>
                    <div className="user__error">
                        <span className="user__error-text">
                            Could not load your user information
                        </span>
                        <FontAwesomeIcon icon={['far', 'frown']} />
                    </div>
                </BodyCard>
            );
        }
        return (
            <EditAccount
                currentUser={user} />
            );
    }
}

export default withRouter(Account);