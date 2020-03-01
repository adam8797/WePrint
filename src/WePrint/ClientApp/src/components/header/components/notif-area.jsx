import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import './notif-area.scss';

export default function NotifArea() {
  return (
    <div className="notif-area">
      <div className="notif-area__name">John Smith</div>
      <div className="notif-area__avatar">
        <img src="http://placekitten.com/100" alt="User Avatar" />
      </div>
      <div className="notif-area__icon">
        <FontAwesomeIcon icon="bars" />
      </div>
    </div>
  );
}
