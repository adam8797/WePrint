import React from 'react';
import { Link } from 'react-router-dom';
import { BodyCard } from '../../components';
import Lorem from './components/lorem';

const HomePage = () => (
  <BodyCard>
    <p>This is the home page</p>
    <p>
      Check out this cool Topic:
      <Link to="/topics/components">Components</Link>
    </p>
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
  </BodyCard>
);

export default HomePage;
