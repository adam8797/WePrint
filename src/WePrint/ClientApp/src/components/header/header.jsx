import React from 'react';
import NotifArea from './components/notif-area';
import './header.scss';

const Header = () => {
  return (
    <header className="header">
      <div className="header__logo">WePrint</div>
      <div className="header__menu-bar">
        <input type="text" name="search" id="search " className="header__search" />
        <div className="header__notif-area">
          <NotifArea />
        </div>
      </div>
    </header>
  );
};

export default Header;
