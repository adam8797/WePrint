module.exports = {
  extends: ['react-app', 'airbnb', 'prettier', 'prettier/react'],
  plugins: ['react', 'jsx-a11y', 'prettier'],
  rules: {
    'class-methods-use-this': 'off', // may want to revisit but very restrictive
    'no-plusplus': 'off',
    'import/no-extraneous-dependencies': 'off',
    'react/jsx-props-no-spreading': 'off',
    'react/require-default-props': [2, { ignoreFunctionalComponents: true }],
    /* prevents divs from having click handlers... common */
    'jsx-a11y/no-static-element-interactions': 'off',
  },
};
