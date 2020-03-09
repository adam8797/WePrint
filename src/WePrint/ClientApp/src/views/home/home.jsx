import React from 'react';
import { Link } from 'react-router-dom';
import { BodyCard, SectionTitle } from '../../components';
import './home.scss';

const HomePage = () => (
  <BodyCard className="homepage">
    <SectionTitle title="Welcome to WePrint" />
    <p>We&apos;re happy you&apos;e here!</p>
    <ul>
      <li>
        If you want to get something printed click{' '}
        <Link to="/post">
          <b>Post a Job</b>
        </Link>{' '}
        on the left
      </li>
      <li>
        If you want to use your printer to make things for people start by adding your device under{' '}
        <Link to="/devices">
          <b>Devices</b>
        </Link>{' '}
        Then check out{' '}
        <Link to="find">
          <b>Find a Job</b>
        </Link>{' '}
        to start placing Bids
      </li>
    </ul>
  </BodyCard>
);

export default HomePage;
