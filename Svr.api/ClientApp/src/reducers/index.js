import claimsLoadedType from '../action-types/claim-types';

const initialState = {
  claims: [
    {
      id: 1,
      title: 'Production-Ready MicroServices',
      author: 'Susan J. Fowler'
    },
    {
      id: 2,
      title: 'Release It!',
      author: 'Michael T. Nygard'
    },
  ],
  columns: [
    {
      dataField: 'title',
      text: 'title',
      sort: true,
      //filter: textFilter()
    },
    {
      dataField: 'author',
      text: 'author',
      sort: true,
      //filter: textFilter()
    },
  ]
};

const reducer = (state = initialState, action) =>
{

  switch (action.type)
  {
    case claimsLoadedType:
      return {
        claims: action.payload.claims,
        columns: action.payload.columns
      };
    default:
      return state;
  }
};

export default reducer;
