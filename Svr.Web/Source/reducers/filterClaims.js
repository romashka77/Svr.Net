const initialState = '';

export default function filterClaims(state = initialState, action) {
  if (action.type === 'FIND_CLAIM') {
    return action.payload;
  }
  return state;
}
