
export default class ClaimStoreService
{
  getClaims()
  {
    return [
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
      {
        id: 3,
        title: 'Production-Ready MicroServices',
        author: 'Susan J. Fowler'
      },
      {
        id: 4,
        title: 'Release It!',
        author: 'Michael T. Nygard'
      },
      {
        id: 5,
        title: 'Production-Ready MicroServices',
        author: 'Susan J. Fowler'
      },
      {
        id: 6,
        title: 'Release It!',
        author: 'Michael T. Nygard'
      },
      {
        id: 7,
        title: 'Production-Ready MicroServices',
        author: 'Susan J. Fowler'
      },
      {
        id: 8,
        title: 'Release It!',
        author: 'Michael T. Nygard'
      },
      {
        id: 9,
        title: 'Production-Ready MicroServices',
        author: 'Susan J. Fowler'
      },
      {
        id: 10,
        title: 'Release It!',
        author: 'Michael T. Nygard'
      },
      {
        id: 11,
        title: 'Production-Ready MicroServices',
        author: 'Susan J. Fowler'
      },
      {
        id: 12,
        title: 'Release It!',
        author: 'Michael T. Nygard'
      },
    ];
  }

  getColumns()
  {
    return [
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
    ];
  }
}