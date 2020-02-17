import React from 'react';
import { useParams } from 'react-router-dom';

function Topic() {
  const { topicId } = useParams();
  return (
    <h3>
      Requested topic ID:
      {` ${topicId}`}
    </h3>
  );
}

export default Topic;
