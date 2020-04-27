import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import UserApi from '../../api/UserApi';

import BodyCard from '../../components/body-card/body-card';
import WepInput from '../../components/wep-input/wep-input';
import WepTextarea from '../../components/wep-textarea/wep-textarea';
import Button, { ButtonType } from '../../components/button/button';

function EditAccount(props) {
    const { currentUser } = props;

    const [firstName, setFirstName] = useState(currentUser.firstName);
    const [lastName, setLastName] = useState(currentUser.lastName);
    const [bio, setBio] = useState(currentUser.bio);

    if (!currentUser) {
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



    const saveUser = () => {
        const newUser = currentUser;
        newUser.firstName = firstName;
        newUser.lastName = lastName;
        newUser.bio = bio;
        UserApi.UpdateUser(newUser).subscribe(() => { }, console.error);
    }

    return (
        <BodyCard>
            <WepInput
                name="firstname"
                id="firstname"
                label="First Name"
                placeholder="First name"
                value={firstName}
                handleChange={ev => setFirstName(ev.target.value)} /><br />
            <WepInput
                name="lastname"
                id="lastname"
                label="Last Name"
                placeholder="Last Name"
                value={lastName}
                handleChange={ev => setLastName(ev.target.value)} /> <br />
            <WepTextarea
                name="bio"
                id="bio"
                label="Bio"
                placeholder="Bio"
                value={bio}
                handleChange={ev => setBio(ev.target.value)} /><br />
            <Button onClick={saveUser} type={ButtonType.SUCCESS} > 
                Save
            </Button>
        </BodyCard>
    );
}

EditAccount.propsTypes = {
    currentUser: PropTypes.objectOf(
        PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object])
    )
};

export default EditAccount;