import React from 'react';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { BodyCard, Button, SectionTitle } from '../../components';
import Lorem from './components/lorem';

const HomePage = () => (
  <BodyCard>
    <SectionTitle title="Home Page" />
    <p>This is the home page</p>
    <p>
      Check out this cool Topic:
      <Link to="/topics/components">Components</Link>
    </p>
    <Button type={Button.Type.PRIMARY}>Do the thing!</Button>
    <FontAwesomeIcon icon="coffee" />
    <FontAwesomeIcon icon={['fab', 'google']} size="lg" />
    <FontAwesomeIcon icon={['fab', 'microsoft']} spin size="lg" />
    <FontAwesomeIcon icon="check-square" bordered size="xs" />
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
    <Lorem />
    <SectionTitle title="Lower Section" />
    <Lorem />
    <Lorem />
  </BodyCard>
);

export default HomePage;
