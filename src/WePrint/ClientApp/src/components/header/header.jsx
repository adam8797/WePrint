import React from 'react';
import { Link } from 'react-router-dom';
import NotifArea from './components/notif-area';
import SearchBar from './components/search-bar';
import './header.scss';

const Header = () => {
  return (
    <header className="header">
      <div className="header__logo">
        <Link to="/">WePrint</Link>
      </div>
      <div className="header__menu-bar">
        <SearchBar />
        <div className="header__notif-area">
          <NotifArea />
        </div>
      </div>
    </header>
  );
};

export default Header;
