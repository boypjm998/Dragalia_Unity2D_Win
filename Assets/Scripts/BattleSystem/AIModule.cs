using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics
{
    [Serializable]
    public class Platform
    {
        public readonly Collider2D collider;

        public Platform(GameObject gameObject)
        {
            collider = gameObject.GetComponentInChildren<Collider2D>();
            //Debug.Log(collider.bounds.max.y);
        }
        public Platform(Collider2D collider)
        {
            this.collider = collider;
        }

        public float height => GetHeight();

        public Vector2 leftBorderPos => GetBorder(-1);

        public Vector2 rightBorderPos => GetBorder();

        private float GetHeight()
        {
            if (collider == null)
                return -999;
            return collider.bounds.max.y;
        }

        private Vector2 GetBorder(int dir = 1)
        {
            if (collider == null)
            {
                if(dir == 1)
                    return new Vector2(999, -999);
                else
                {
                    return new Vector2(-999, -999);
                }
            }

            if (dir == 1)
                return collider.bounds.max;
            //return the right lower corner
            return new Vector2(collider.bounds.min.x, collider.bounds.max.y);
        }

        public static bool CanReach(Platform from, Platform to, float gravity, float jumpSpeed, float horizontalSpeed,
            int maxJumpTimes = 2)
        {
            var singleJumpHeight = jumpSpeed * jumpSpeed / (2 * gravity * 10f);
            var maxJumpHeight = maxJumpTimes * singleJumpHeight;
            var singleJumpTimeToTopPoint = jumpSpeed / (10f * gravity);

            var distanceY = to.height - from.height;
            float distanceX = 99999;
            //有问题
            
            // distanceX =
            //     Mathf.Min(to.leftBorderPos.x - from.rightBorderPos.x,
            //         from.leftBorderPos.x - to.rightBorderPos.x);
            

            if(distanceY >= maxJumpHeight)
                return false;

            //to在from左边，需要往左跳
            if (from.leftBorderPos.x > to.rightBorderPos.x)
            {
                distanceX = from.leftBorderPos.x - to.rightBorderPos.x;
            }else if (to.leftBorderPos.x > from.rightBorderPos.x)
            {
                //to在from右边，需要往右跳
                distanceX = to.leftBorderPos.x - from.rightBorderPos.x;
            }
            else
            {
                return true;
            }
            

            if (distanceY < maxJumpHeight)
            {
                //When the platform is below me, need to re-calculating.

                if (distanceY < 0)
                {
                    var ascendTime = jumpSpeed / (10f * gravity);
                    var ascendHeight = 0.5f * gravity * ascendTime * ascendTime;
                    var smallJumpTime = ascendTime * 2;
                    var fallDistance = (ascendHeight + Mathf.Abs(distanceY));
                    var fallTime = Mathf.Sqrt(2 * fallDistance / (gravity * 10f));
                    
                    var maxMoveDistanceX = horizontalSpeed * (smallJumpTime * (maxJumpTimes-1) + fallTime + ascendTime);
                    if(maxMoveDistanceX + 1 > distanceX)
                        return true;
                    else
                    {
                        //Debug.Log("distanceY: "+distanceY+"fallTime: "+fallTime);
                        //Debug.Log("distanceX: "+distanceX+" maxMoveDistanceX: "+maxMoveDistanceX);
                        return false;
                    }
                }
                else
                {
                    
                     var timeToFallDown = Mathf.Sqrt(2 * (maxJumpHeight - distanceY) / (gravity * 10f));
                     var maxMoveDistanceX = horizontalSpeed * (maxJumpTimes * singleJumpTimeToTopPoint + timeToFallDown);
                     if (maxMoveDistanceX + 1f > distanceX) //1:collider补正
                         return true;
                    
                     
                     return false;
                }



                
            }

            return false;
        }
        
        public static bool CanReach(Vector3 fromPos, Platform to, float gravity, float jumpSpeed, float horizontalSpeed,
            int maxJumpTimes = 2)
        {
            var singleJumpHeight = jumpSpeed * jumpSpeed / (2 * gravity * 10f);
            var maxJumpHeight = maxJumpTimes * singleJumpHeight;
            var singleJumpTimeToTopPoint = jumpSpeed / (10f * gravity);

            var distanceY = to.height - fromPos.y;
            float distanceX = 99999;
            //有问题
            
            // distanceX =
            //     Mathf.Min(to.leftBorderPos.x - from.rightBorderPos.x,
            //         from.leftBorderPos.x - to.rightBorderPos.x);
            

            if(distanceY >= maxJumpHeight)
                return false;

            //to在from左边，需要往左跳
            if (fromPos.x > to.rightBorderPos.x)
            {
                distanceX = fromPos.x - to.rightBorderPos.x;
            }else if (to.leftBorderPos.x > fromPos.x)
            {
                //to在from右边，需要往右跳
                distanceX = to.leftBorderPos.x - fromPos.x;
            }
            else return true;
            
            

            if (distanceY < maxJumpHeight)
            {
                //When the platform is below me, need to re-calculating.

                if (distanceY < 0)
                {
                    var ascendTime = jumpSpeed / (10f * gravity);
                    var ascendHeight = 0.5f * gravity * ascendTime * ascendTime;
                    var smallJumpTime = ascendTime * 2;
                    var fallDistance = (ascendHeight + Mathf.Abs(distanceY));
                    var fallTime = Mathf.Sqrt(2 * fallDistance / (gravity * 10f));
                    
                    var maxMoveDistanceX = horizontalSpeed * (smallJumpTime * (maxJumpTimes-1) + fallTime + ascendTime);
                    if(maxMoveDistanceX + 1 > distanceX)
                        return true;
                    else
                    {
                        //Debug.Log("distanceY: "+distanceY+"fallTime: "+fallTime);
                        //Debug.Log("distanceX: "+distanceX+" maxMoveDistanceX: "+maxMoveDistanceX);
                        return false;
                    }
                }
                else
                {
                    
                     var timeToFallDown = Mathf.Sqrt(2 * (maxJumpHeight - distanceY) / (gravity * 10f));
                     var maxMoveDistanceX = horizontalSpeed * (maxJumpTimes * singleJumpTimeToTopPoint + timeToFallDown);
                     if (maxMoveDistanceX + 1f > distanceX) //1:collider补正
                         return true;
                    
                     Debug.Log("distanceY: "+distanceY);
                     return false;
                }



                
            }

            return false;
        }

        /// <summary>
        ///     Calculate H Value.(计算目前节点到终点的距离估计值)
        /// </summary>
        /// <returns></returns>
        public static float EstimateArriveTime(APlatformNode startNode, APlatformNode destNode, float gravity,
            float jumpSpeed, float horizontalSpeed)
        {

            if (startNode.platform.collider == destNode.platform.collider)
                return 0;
            
            Vector2 startPos, destPos;
            
            
            if (startNode.parent != null)
            {
                //估算落脚点
                if (startNode.parent.pos.x < startNode.platform.leftBorderPos.x)
                {
                    startPos = startNode.platform.leftBorderPos;
                } else if (startNode.parent.pos.x >= startNode.platform.leftBorderPos.x &&
                          startNode.parent.pos.x < startNode.platform.rightBorderPos.x)
                {
                    startPos = new Vector2(startNode.parent.pos.x,startNode.platform.height);
                }
                else
                {
                    startPos = startNode.platform.rightBorderPos;
                }

            }
            else
            {
                startPos = new Vector2(startNode.pos.x,destNode.pos.y); //刚刚登上平台的位置
            }



            destPos = destNode.pos; //目标平台的位置
            var horiziontalMoveTime = Mathf.Abs(destPos.x - startPos.x) / horizontalSpeed;
            float verticalMoveTime;
            if (startPos.y > destPos.y)
            {
                verticalMoveTime = Mathf.Sqrt(2 * (startPos.y - destPos.y) / (gravity * 10f));
            }
            else
            {
                var distanceY = destPos.y - startPos.y;
                var singleJumpHeight = jumpSpeed * jumpSpeed / (2 * gravity * 10f);
                var singleJumpTimeEstimate = jumpSpeed / (10f * gravity);

                var remainderDistanceY = distanceY % singleJumpHeight;
                var jumpTimesRequire = Mathf.CeilToInt(distanceY / singleJumpHeight);
                
                var fallDistance = singleJumpHeight - remainderDistanceY;

                var timeToFallDown = Mathf.Sqrt((2 * fallDistance) / (gravity * 10f));

                float fallPunishment = jumpTimesRequire*jumpTimesRequire*0.1f;
                fallPunishment = Mathf.Min(fallPunishment, 1f);
                

                verticalMoveTime = (singleJumpTimeEstimate) * (jumpTimesRequire) + timeToFallDown;
                
                
                
                
            }
            

            return Mathf.Max(verticalMoveTime, horiziontalMoveTime);
        }
        
    }

    [Serializable]
    public class AStar
    {
        public List<APlatformNode> closeList;
        public APlatformNode endNode;
        private float gravity;
        private float horizontalSpeed;
        private float jumpSpeed;
        private float maxJumpTimes = 2;
        public List<APlatformNode> openList;
        public APlatformNode startNode;
        public List<APlatformNode> mapNodes;

        public AStar(float gravity, float jumpSpeed, float horizontalSpeed, float maxJumpTimes = 2)
        {
            this.gravity = gravity;
            this.jumpSpeed = jumpSpeed;
            this.horizontalSpeed = horizontalSpeed;
            this.maxJumpTimes = maxJumpTimes;
        }

        public void Init(List<Platform> map)
        {
            mapNodes = new List<APlatformNode>();
            foreach (var platform in map)
            {
                mapNodes.Add(new APlatformNode(platform));
            }
        }
        
        

        /// <summary>
        /// </summary>
        /// <param name="myMoveInfo">
        ///     Should be a float array with length 4,
        ///     Represent the Gravity, JumpSpd, MoveSpd, MaxJumpTimes
        /// </param>
        /// <returns></returns>
        public List<APlatformNode> Execute(List<Platform> map, Transform me, Transform target)
        {
            bool err = false;
            mapNodes = new List<APlatformNode>();
            foreach (var platform in map)
            {
                mapNodes.Add(new APlatformNode(platform));
            }
            openList = new List<APlatformNode>();
            closeList = new List<APlatformNode>();

            var mySensor = me.GetComponentInChildren<IGroundSensable>();
            var targetSensor = target.GetComponentInChildren<IGroundSensable>();

            var startPlatform = new Platform(BasicCalculation.CheckRaycastedPlatform(me.gameObject));
            var endPlatform = new Platform(BasicCalculation.CheckRaycastedPlatform(target.gameObject));
            
            
            endNode = new APlatformNode(endPlatform);
            startNode = new APlatformNode(startPlatform);

            startNode.g = 0;
            startNode.pos = me.position;
            startNode.parent = null;
            endNode.pos = target.position;
            startNode.h = Platform.EstimateArriveTime(startNode, endNode, gravity, jumpSpeed, horizontalSpeed);
            

            

            
            
            if(startNode.platform.collider == endNode.platform.collider)
            {
                Debug.Log("Start and End are the same platform");
                return null;
            }

            openList.Add(startNode);

            var currentNode = startNode;

            while (openList.Count > 0)
            {
                currentNode = GetLowestFNode(currentNode);
                if (currentNode == null)
                {
                    err = true;
                    Debug.LogException(new Exception("No Path Found"));
                    break;
                }

                
                closeList.Add(currentNode);
                openList.Remove(currentNode);
                //Debug.Log(endNode.platform.collider.gameObject.name);
                if(APlatformNode.InSameCollider(endNode,closeList))
                {
                    endNode.parent = currentNode;
                    break;
                }

                
                //Path found
                var reachablePlatforms = GetAllReachableNodes(currentNode,mapNodes);

                foreach (var platform in reachablePlatforms)
                {
                    if(APlatformNode.InSameCollider(platform,closeList)) continue;

                    
                    //Calculate the G,H,F value, Set Parent
                    platform.parent = currentNode;
                    GetNodeValues(platform);
                    openList.Add(platform);

                }
            }
            
            
            
            
            List<APlatformNode> path = new List<APlatformNode>();
            APlatformNode lastNode = endNode;
            path.Add(lastNode);
            if(lastNode.parent == null)
            {
                Debug.Log("No Path Found");
                return null;
            }
            while (lastNode.platform.collider != startNode.platform.collider)
            {
                //Debug.Log(endNode.parent.platform.collider.name + "----->");
                path.Add(lastNode.parent);
                lastNode = lastNode.parent;
            }
            path.RemoveAt(0);
            path.Reverse();
            
            
            //输出path的长度
            
            //按顺序输出路径
            foreach (var node in path)
            {
                
            }
            return path;
        }
        
        public APlatformNode GetAPlatformNode(Collider2D col)
        {
            foreach (var node in mapNodes)
            {
                if (node.platform.collider == col)
                {
                    return node;
                }
            }

            return null;
        }

        private APlatformNode GetLowestFNode(APlatformNode currentNode)
        {
            var reachables = GetAllReachableNodes(currentNode);

            //Find the lowest F node
            APlatformNode lowestFNode = null;
            foreach (var node in reachables)
                if (lowestFNode == null)
                {
                    lowestFNode = node;
                }
                else
                {
                    if (node.f < lowestFNode.f) lowestFNode = node;
                    if (node.f == lowestFNode.f && node.pos.y >= lowestFNode.pos.y)
                    {
                        lowestFNode = node;
                    }

                }

            while (lowestFNode == null)
            {
                if(currentNode.parent == null) return null;
                closeList.Add(currentNode);
                currentNode = currentNode.parent;
                return GetLowestFNode(currentNode);
            }

                //估算落脚点
            if (currentNode.pos.x < lowestFNode.platform.leftBorderPos.x)
            {
                lowestFNode.pos = lowestFNode.platform.leftBorderPos;
            } else if(currentNode.pos.x >= lowestFNode.platform.leftBorderPos.x &&
                      currentNode.pos.x < lowestFNode.platform.rightBorderPos.x)
            {
                //lowestFNode.pos = currentNode.pos;
                lowestFNode.pos = new Vector2(currentNode.pos.x,lowestFNode.platform.height);
            }
            else
            {
                lowestFNode.pos = lowestFNode.platform.rightBorderPos;
            }
            // Debug.Log("选择了"+lowestFNode.platform.collider.name+"作为下一个节点");
            //if(lowestFNode.platform.collider.name == "Platform5")
                //Debug.Log("选择了"+lowestFNode.platform.collider.name+"作为下一个节点");

            return lowestFNode;
        }

        /// <summary>
        /// 寻找OpenList中所有可以到达的Node
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private List<APlatformNode> GetAllReachableNodes(APlatformNode currentNode,List<APlatformNode> mapNodes)
        {
            var reachables = new List<APlatformNode>();
            foreach (var node in mapNodes)
            {
                if(APlatformNode.InSameCollider(node,closeList))
                    continue;

                if(currentNode.platform.collider == node.platform.collider)
                    continue;
                
                if (Platform.CanReach(currentNode.platform, node.platform, gravity, jumpSpeed, horizontalSpeed,(int)maxJumpTimes))
                    reachables.Add(node);
                
            }
            
            return reachables;
        }
        private List<APlatformNode> GetAllReachableNodes(APlatformNode currentNode)
        {
            var reachables = new List<APlatformNode>();
            foreach (var node in openList)
            {
                if(APlatformNode.InSameCollider(node,closeList))
                    continue;

                if (Platform.CanReach(currentNode.platform, node.platform, gravity, jumpSpeed, horizontalSpeed,(int)maxJumpTimes))
                    reachables.Add(node);
                
            }
            
            return reachables;
        }

        private void GetNodeValues(APlatformNode node)
        {
            
                node.h = Platform.EstimateArriveTime
                    (node, endNode , gravity, jumpSpeed, horizontalSpeed);
                node.g += APlatformNode.CalculateArriveTime(node, gravity, jumpSpeed, horizontalSpeed);
                //Debug.Log("H value from " + node.platform.collider.name + " to " + endNode.platform.collider.name + " is " + node.h);
                //Debug.Log("G value is " + node.g);
                //Debug.Log("F value from " + node.platform.collider.name + " to " + endNode.platform.collider.name + " is " + node.f);
        }
    }

    [Serializable]
    public class APlatformNode
    {
        public float g;
        public float h;
        public APlatformNode parent;
        public Platform platform;

        /// <summary>
        ///     人物在该平台上的落脚点
        /// </summary>
        public Vector2 pos;

        public APlatformNode(Platform platform, APlatformNode parent, float g, float h, Vector2 pos)
        {
            this.platform = platform;
            this.parent = parent;
            this.g = g;
            this.h = h;
            this.pos = pos;
        }

        public APlatformNode(Platform platform)
        {
            this.platform = platform;
        }

        public float f => g + h;

        /// <summary>
        /// 已知目标起点和终点，以及人物的移动速度，重力加速度，跳跃速度，计算出人物到达终点的时间。
        /// </summary>
        public static float CalculateArriveTime(APlatformNode endNode, float gravity,
            float jumpSpeed, float horizontalSpeed)
        {
            float finalMoveTime;

            var parent = endNode.parent;
            var startPos = parent.pos;
            Vector2 endPos;
            
            //估算落脚点
            if (parent.pos.x < endNode.platform.leftBorderPos.x)
            {
                endPos = endNode.platform.leftBorderPos;
            } else if(parent.pos.x >= endNode.platform.leftBorderPos.x &&
                           parent.pos.x < endNode.platform.rightBorderPos.x)
            {
                //endPos = parent.pos;
                endPos = new Vector2(parent.pos.x,endNode.platform.height);
            }
            else
            {
                endPos = endNode.platform.rightBorderPos;
            }

            
            


            

            var distanceY = endPos.y - startPos.y;
            var distanceX = Mathf.Abs(endPos.x - startPos.x);
            
            if (distanceY < 0)
            {
                //下落
                var dropTime = Mathf.Sqrt(2 * Mathf.Abs(distanceY) / (gravity * 10f));
                //在坠落期间，水平移动的距离
                var moveDistanceXDuringFall = dropTime * horizontalSpeed;
                
                //如果下落中能够移动的距离小于水平已经移动了的距离，说明角色在X轴方向是持续移动的，最终时间将以x方向移动时间为准
                if(moveDistanceXDuringFall < distanceX)
                {
                   //如果间隙大于下落中水平移动的距离，说明在下落过程中进行了跳跃。
                   //在这种情况下，竖直方向的移动时间将没有意义，因为下落过程中角色的水平移动速度不会被影响。
                   //所以，只需要计算水平方向的移动时间即可。
                   var moveTime = distanceX / horizontalSpeed;
                   finalMoveTime = moveTime;
                }
                else
                {
                    //如果间隙小于下落中水平移动的距离，说明在下落过程中没有进行跳跃。
                    //在这种情况下，总移动时间为自由落体时间。
                    var moveTime = dropTime;
                    finalMoveTime = moveTime;
                }

            }
            else
            {
                //上升
                //获取一次跳跃所能上升的最大高度
                var maxSingleJumpHeight = jumpSpeed * jumpSpeed / (2 * gravity * 10f);
                //获取到达distanceY所需要的跳跃次数
                var jumpTimes = Mathf.CeilToInt(distanceY / maxSingleJumpHeight);
                //获取跳跃jumpTimes-1次后，角色距离终点的高度
                var heightAfterJump = distanceY - (jumpTimes - 1) * maxSingleJumpHeight;
                //获取跳跃一次的时间
                var singleJumpTime = jumpSpeed / (gravity * 10f);
                //获取自由落体heightAfterJump距离所需的时间
                var dropTime = Mathf.Sqrt(2 * heightAfterJump / (gravity * 10f));
                //总跳跃时间
                var totalAirTime = jumpTimes * singleJumpTime + dropTime;
                
                //在空中，能够进行水平移动的距离
                var moveDistanceXInAir = totalAirTime * horizontalSpeed;
                
                if (distanceX < moveDistanceXInAir)
                {
                    //如果实际移动的距离小于能够移动的距离，说明目标在提前到达X轴方向的目标点，那么总移动时间以Y轴方向的移动时间为准
                    finalMoveTime = totalAirTime;
                }
                else
                {
                    //如果实际移动的距离大于能够移动的距离，说明目标在起跳之前进行了一段x轴方向的移动，那么总移动时间以X轴方向的移动时间为准。
                    var moveTime = distanceX / horizontalSpeed;
                    finalMoveTime = moveTime;
                }

            }
            
            return finalMoveTime;

        }
        
        public static float CalculateArriveTime(Vector2 startPos, APlatformNode endNode, float gravity,
            float jumpSpeed, float horizontalSpeed)
        {
            float finalMoveTime;

            //var parent = endNode.parent;
         
            Vector2 endPos;

            //估算落脚点
            if (startPos.x < endNode.platform.leftBorderPos.x)
            {
                endPos = endNode.platform.leftBorderPos;
            }
            else if (startPos.x >= endNode.platform.leftBorderPos.x &&
                         startPos.x < endNode.platform.rightBorderPos.x)
            {
                //endPos = parent.pos;
                endPos = new Vector2(startPos.x, endNode.platform.height);
            }
            else
            {
                endPos = endNode.platform.rightBorderPos;
            }


            var distanceY = endPos.y - startPos.y;
            var distanceX = Mathf.Abs(endPos.x - startPos.x);

            if (distanceY < 0)
            {
                //下落
                var dropTime = Mathf.Sqrt(2 * Mathf.Abs(distanceY) / (gravity * 10f));
                //在坠落期间，水平移动的距离
                var moveDistanceXDuringFall = dropTime * horizontalSpeed;

                //如果下落中能够移动的距离小于水平已经移动了的距离，说明角色在X轴方向是持续移动的，最终时间将以x方向移动时间为准
                if (moveDistanceXDuringFall < distanceX)
                {
                    //如果间隙大于下落中水平移动的距离，说明在下落过程中进行了跳跃。
                    //在这种情况下，竖直方向的移动时间将没有意义，因为下落过程中角色的水平移动速度不会被影响。
                    //所以，只需要计算水平方向的移动时间即可。
                    var moveTime = distanceX / horizontalSpeed;
                    finalMoveTime = moveTime;
                }
                else
                {
                    //如果间隙小于下落中水平移动的距离，说明在下落过程中没有进行跳跃。
                    //在这种情况下，总移动时间为自由落体时间。
                    var moveTime = dropTime;
                    finalMoveTime = moveTime;
                }

            }
            else
            {
                //上升
                //获取一次跳跃所能上升的最大高度
                var maxSingleJumpHeight = jumpSpeed * jumpSpeed / (2 * gravity * 10f);
                //获取到达distanceY所需要的跳跃次数
                var jumpTimes = Mathf.CeilToInt(distanceY / maxSingleJumpHeight);
                //获取跳跃jumpTimes-1次后，角色距离终点的高度
                var heightAfterJump = distanceY - (jumpTimes - 1) * maxSingleJumpHeight;
                //获取跳跃一次的时间
                var singleJumpTime = jumpSpeed / (gravity * 10f);
                //获取自由落体heightAfterJump距离所需的时间
                var dropTime = Mathf.Sqrt(2 * heightAfterJump / (gravity * 10f));
                //总跳跃时间
                var totalAirTime = jumpTimes * singleJumpTime + dropTime;

                //在空中，能够进行水平移动的距离
                var moveDistanceXInAir = totalAirTime * horizontalSpeed;

                if (distanceX < moveDistanceXInAir)
                {
                    //如果实际移动的距离小于能够移动的距离，说明目标在提前到达X轴方向的目标点，那么总移动时间以Y轴方向的移动时间为准
                    finalMoveTime = totalAirTime;
                }
                else
                {
                    //如果实际移动的距离大于能够移动的距离，说明目标在起跳之前进行了一段x轴方向的移动，那么总移动时间以X轴方向的移动时间为准。
                    var moveTime = distanceX / horizontalSpeed;
                    finalMoveTime = moveTime;
                }

            }

            return finalMoveTime;

        }

       


        public static bool InSameCollider(APlatformNode node,List<APlatformNode> closeList)
        {
            foreach (var closeNode in closeList)
            {
                if (closeNode.platform.collider == node.platform.collider)
                    return true;
            }

            return false;
        }

        


    }
}