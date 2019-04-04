const initialState = [
  {
    id: 1,
    name: 'region1',
  },
  {
    id: 2,
    name: 'region2',
  },
  {
    id: 3,
    name: 'region4',
  },
];

export default function regions(state = initialState, action) {
  if (action.type === 'ADD_REGION') {
    return state;
  } if (action.type === 'DELETE_REGION') {
    return state;
  }
  return state;
}
