using System;
using System.Collections.Generic;
using System.Text;

namespace YearInReview
{
    class ProductHuntPost
    {
        public Posts posts { get; set; }
    }

    class Posts
    {
        public List<Edges> edges { get; set; }

        public PageInfo pageInfo { get; set; }
    }

    class PageInfo
    {
        public string endCursor { get; set; }

        public bool hasNextPage { get; set; }
    }

    class Edges
    {
        public Node node { get; set; }

        public string cursor { get; set; }
    }

    class Node
    {
        public string name { get; set; }

        public string slug { get; set; }

        public string website { get; set; }

        public string url { get; set; }

        public string description { get; set; }

        public string tagline { get; set; }

        public int votesCount { get; set; }

        public int commentsCount { get; set; }
    }
}

//{
//    "data": {
//        "posts": {
//            "edges": [
//                {
//                    "node": {
//                        "name": "What to Tweet",
//                        "slug": "what-to-tweet",
//                        "website": "https://www.producthunt.com/r/6ce47c0311c4b4?utm_campaign=producthunt-api&utm_medium=api-v2&utm_source=Application%3A+Report+%28ID%3A+41362%29",
//                        "url": "https://www.producthunt.com/posts/what-to-tweet?utm_campaign=producthunt-api&utm_medium=api-v2&utm_source=Application%3A+Report+%28ID%3A+41362%29",
//                        "createdAt": "2020-12-14T08:00:00Z"
//                    },
//                    "cursor": "MQ=="
//                },
//            ],
//            "pageInfo": {
//                "endCursor": "MjA=",
//                "hasNextPage": false
//            }
//        }
//    }
//}