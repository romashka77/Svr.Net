const initialState = '';

export default (state = initialState, action) => {
  if (action.type === 'FIND_CLAIM') {
    return action.payload;
  }
  return state;
};
