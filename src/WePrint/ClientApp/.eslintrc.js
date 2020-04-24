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
    'jsx-a11y/label-has-associated-control': [
      2,
      {
        controlComponents: ['WepInput', 'WepTextarea', 'WepDropdown'],
        depth: 3,
      },
    ],
    'jsx-a11y/click-events-have-key-events': 'off', // too restrictive for now, remove when we do an a11y pass
  },
};
