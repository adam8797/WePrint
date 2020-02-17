module.exports = {
  extends: ['react-app', 'airbnb', 'prettier', 'prettier/react'],
  plugins: ['react', 'jsx-a11y', 'prettier'],
  rules: {
    'class-methods-use-this': 'off', // may want to revisit but very restrictive
    'no-plusplus': 'off',
    'import/no-extraneous-dependencies': 'off',
  },
};
